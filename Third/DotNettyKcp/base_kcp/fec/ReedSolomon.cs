using System;

namespace fec;

public class ReedSolomon
{
    private readonly CodingLoop codingLoop;

    /**
         * Initializes a new encoder/decoder, with a chosen coding loop.
         */
    public ReedSolomon(int dataShardCount, int parityShardCount, CodingLoop codingLoop)
    {
        // We can have at most 256 shards total, as any more would
        // lead to duplicate rows in the Vandermonde matrix, which
        // would then lead to duplicate rows in the built matrix
        // below. Then any subset of the rows containing the duplicate
        // rows would be singular.
        if (256 < dataShardCount + parityShardCount) throw new Exception("too many shards - max is 256");

        DataShardCount = dataShardCount;
        ParityShardCount = parityShardCount;
        this.codingLoop = codingLoop;
        TotalShardCount = dataShardCount + parityShardCount;
        Matrix = buildMatrix(dataShardCount, TotalShardCount);
        ParityRows = new byte [parityShardCount][];
        for (var i = 0; i < parityShardCount; i++) ParityRows[i] = Matrix.getRow(dataShardCount + i);
    }

    public int DataShardCount { get; }

    public int ParityShardCount { get; }

    public int TotalShardCount { get; }

    /**
         * Rows from the matrix for encoding parity, each one as its own
         * byte array to allow for efficient access while encoding.
         */
    public byte[][] ParityRows { get; }

    public Matrix Matrix { get; }

    /**
         * Creates a ReedSolomon codec with the default coding loop.
         */
    public static ReedSolomon create(int dataShardCount, int parityShardCount)
    {
        return new ReedSolomon(dataShardCount, parityShardCount, new InputOutputByteTableCodingLoop());
    }

    /**
         * Returns the number of data shards.
         */
    public int getDataShardCount()
    {
        return DataShardCount;
    }

    /**
         * Returns the number of parity shards.
         */
    public int getParityShardCount()
    {
        return ParityShardCount;
    }

    /**
         * Returns the total number of shards.
         */
    public int getTotalShardCount()
    {
        return TotalShardCount;
    }

    /**
         * Encodes parity for a set of data shards.
         * 
         * @param shards An array containing data shards followed by parity shards.
         * Each shard is a byte array, and they must all be the same
         * size.
         * @param offset The index of the first byte in each shard to encode.
         * @param byteCount The number of bytes to encode in each shard.
         */
    public void encodeParity(byte[][] shards, int offset, int byteCount)
    {
        // Check arguments.
        checkBuffersAndSizes(shards, offset, byteCount);

        // Build the array of output buffers.
        var outputs = new byte [ParityShardCount][];
        Array.Copy(shards, DataShardCount, outputs, 0, ParityShardCount);

        // Do the coding.
        codingLoop.codeSomeShards(
            ParityRows,
            shards, DataShardCount,
            outputs, ParityShardCount,
            offset, byteCount);
    }


    /**
         * Returns true if the parity shards contain the right data.
         * 
         * @param shards An array containing data shards followed by parity shards.
         * Each shard is a byte array, and they must all be the same
         * size.
         * @param firstByte The index of the first byte in each shard to check.
         * @param byteCount The number of bytes to check in each shard.
         */
    public bool isParityCorrect(byte[][] shards, int firstByte, int byteCount)
    {
        // Check arguments.
        checkBuffersAndSizes(shards, firstByte, byteCount);

        // Build the array of buffers being checked.
        var toCheck = new byte [ParityShardCount][];
        Array.Copy(shards, DataShardCount, toCheck, 0, ParityShardCount);


        // Do the checking.
        return codingLoop.checkSomeShards(
            ParityRows,
            shards, DataShardCount,
            toCheck, ParityShardCount,
            firstByte, byteCount,
            null);
    }

    /**
         * Returns true if the parity shards contain the right data.
         * 
         * This method may be significantly faster than the one above that does
         * not use a temporary buffer.
         * 
         * @param shards An array containing data shards followed by parity shards.
         * Each shard is a byte array, and they must all be the same
         * size.
         * @param firstByte The index of the first byte in each shard to check.
         * @param byteCount The number of bytes to check in each shard.
         * @param tempBuffer A temporary buffer (the same size as each of the
         * shards) to use when computing parity.
         */
    public bool isParityCorrect(byte[][] shards, int firstByte, int byteCount, byte[] tempBuffer)
    {
        // Check arguments.
        checkBuffersAndSizes(shards, firstByte, byteCount);
        if (tempBuffer.Length < firstByte + byteCount) throw new Exception("tempBuffer is not big enough");

        // Build the array of buffers being checked.
        var toCheck = new byte [ParityShardCount][];
        Array.Copy(shards, DataShardCount, toCheck, 0, ParityShardCount);

        // Do the checking.
        return codingLoop.checkSomeShards(
            ParityRows,
            shards, DataShardCount,
            toCheck, ParityShardCount,
            firstByte, byteCount,
            tempBuffer);
    }


    /**
         * Given a list of shards, some of which contain data, fills in the
         * ones that don't have data.
         *
         * Quickly does nothing if all of the shards are present.
         *
         * If any shards are missing (based on the flags in shardsPresent),
         * the data in those shards is recomputed and filled in.
         */
    public void decodeMissing(byte[][] shards,
        bool[] shardPresent,
        int offset,
        int byteCount)
    {
        // Check arguments.
        checkBuffersAndSizes(shards, offset, byteCount);

        // Quick check: are all of the shards present?  If so, there's
        // nothing to do.
        var numberPresent = 0;
        for (var i = 0; i < TotalShardCount; i++)
            if (shardPresent[i])
                numberPresent += 1;

        if (numberPresent == TotalShardCount)
            // Cool.  All of the shards data data.  We don't
            // need to do anything.
            return;

        // More complete sanity check
        if (numberPresent < DataShardCount) throw new Exception("Not enough shards present");

        // Pull out the rows of the matrix that correspond to the
        // shards that we have and build a square matrix.  This
        // matrix could be used to generate the shards that we have
        // from the original data.
        //
        // Also, pull out an array holding just the shards that
        // correspond to the rows of the submatrix.  These shards
        // will be the input to the decoding process that re-creates
        // the missing data shards.
        var subMatrix = new Matrix(DataShardCount, DataShardCount);
        var subShards = new byte [DataShardCount][];
        {
            var subMatrixRow = 0;
            for (var matrixRow = 0; matrixRow < TotalShardCount && subMatrixRow < DataShardCount; matrixRow++)
                if (shardPresent[matrixRow])
                {
                    for (var c = 0; c < DataShardCount; c++)
                        subMatrix.set(subMatrixRow, c, Matrix.get(matrixRow, c));

                    subShards[subMatrixRow] = shards[matrixRow];
                    subMatrixRow += 1;
                }
        }

        // Invert the matrix, so we can go from the encoded shards
        // back to the original data.  Then pull out the row that
        // generates the shard that we want to decode.  Note that
        // since this matrix maps back to the orginal data, it can
        // be used to create a data shard, but not a parity shard.
        var dataDecodeMatrix = subMatrix.invert();

        // Re-create any data shards that were missing.
        //
        // The input to the coding is all of the shards we actually
        // have, and the output is the missing data shards.  The computation
        // is done using the special decode matrix we just built.
        var outputs = new byte [ParityShardCount][];
        var matrixRows = new byte [ParityShardCount][];
        var outputCount = 0;
        for (var iShard = 0; iShard < DataShardCount; iShard++)
            if (!shardPresent[iShard])
            {
                outputs[outputCount] = shards[iShard];
                matrixRows[outputCount] = dataDecodeMatrix.getRow(iShard);
                outputCount += 1;
            }

        codingLoop.codeSomeShards(
            matrixRows,
            subShards, DataShardCount,
            outputs, outputCount,
            offset, byteCount);

        // Now that we have all of the data shards intact, we can
        // compute any of the parity that is missing.
        //
        // The input to the coding is ALL of the data shards, including
        // any that we just calculated.  The output is whichever of the
        // data shards were missing.
        outputCount = 0;
        for (var iShard = DataShardCount; iShard < TotalShardCount; iShard++)
            if (!shardPresent[iShard])
            {
                outputs[outputCount] = shards[iShard];
                matrixRows[outputCount] = ParityRows[iShard - DataShardCount];
                outputCount += 1;
            }

        codingLoop.codeSomeShards(
            matrixRows,
            shards, DataShardCount,
            outputs, outputCount,
            offset, byteCount);
    }

    /**
         * Checks the consistency of arguments passed to public methods.
         */
    private void checkBuffersAndSizes(byte[][] shards, int offset, int byteCount)
    {
        // The number of buffers should be equal to the number of
        // data shards plus the number of parity shards.
        if (shards.Length != TotalShardCount) throw new Exception("wrong number of shards: " + shards.Length);

        // All of the shard buffers should be the same length.
        var shardLength = 0;
        var allShardIsEmpty = true;
        //int shardLength = shards[0].length;
        for (var i = 1; i < shards.Length; i++)
        {
            if (shards[i] == null)
                continue;
            allShardIsEmpty = false;
            if (shardLength == 0)
            {
                shardLength = shards[i].Length;
                continue;
            }

            if (shards[i].Length != shardLength) throw new Exception("Shards are different sizes");
        }

        if (allShardIsEmpty) throw new Exception("Shards are empty");

        // The offset and byteCount must be non-negative and fit in the buffers.
        if (offset < 0) throw new Exception("offset is negative: " + offset);

        if (byteCount < 0) throw new Exception("byteCount is negative: " + byteCount);

        if (shardLength < offset + byteCount) throw new Exception("buffers to small: " + byteCount + offset);
    }

    /**
         * Create the matrix to use for encoding, given the number of
         * data shards and the number of total shards.
         *
         * The top square of the matrix is guaranteed to be an identity
         * matrix, which means that the data shards are unchanged after
         * encoding.
         */
    private static Matrix buildMatrix(int dataShards, int totalShards)
    {
        // Start with a Vandermonde matrix.  This matrix would work,
        // in theory, but doesn't have the property that the data
        // shards are unchanged after encoding.
        var matrix = vandermonde(totalShards, dataShards);

        // Multiple by the inverse of the top square of the matrix.
        // This will make the top square be the identity matrix, but
        // preserve the property that any square subset of rows is
        // invertible.
        var top = matrix.submatrix(0, 0, dataShards, dataShards);
        return matrix.times(top.invert());
    }

    /**
         * Create a Vandermonde matrix, which is guaranteed to have the
         * property that any subset of rows that forms a square matrix
         * is invertible.
         *
         * @param rows Number of rows in the result.
         * @param cols Number of columns in the result.
         * @return A Matrix.
         */
    private static Matrix vandermonde(int rows, int cols)
    {
        var result = new Matrix(rows, cols);
        try
        {
            for (var r = 0; r < rows; r++)
            for (var c = 0; c < cols; c++)
                result.set(r, c, Galois.exp((byte) r, c));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return result;
    }
}