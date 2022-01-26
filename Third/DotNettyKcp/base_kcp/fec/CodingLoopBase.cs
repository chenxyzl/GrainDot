namespace fec;

public abstract class CodingLoopBase : CodingLoop
{
    public abstract void codeSomeShards(byte[][] matrixRows, byte[][] inputs, int inputCount, byte[][] outputs,
        int outputCount,
        int offset, int byteCount);

    public virtual bool checkSomeShards(
        byte[][] matrixRows,
        byte[][] inputs, int inputCount,
        byte[][] toCheck, int checkCount,
        int offset, int byteCount,
        byte[] tempBuffer)
    {
        // This is the loop structure for ByteOutputInput, which does not
        // require temporary buffers for checking.
        var table = Galois.MULTIPLICATION_TABLE;
        for (var iByte = offset; iByte < offset + byteCount; iByte++)
        for (var iOutput = 0; iOutput < checkCount; iOutput++)
        {
            var matrixRow = matrixRows[iOutput];
            var value = 0;
            for (var iInput = 0; iInput < inputCount; iInput++)
                value ^= table[matrixRow[iInput] & 0xFF][inputs[iInput][iByte] & 0xFF];

            if (toCheck[iOutput][iByte] != value) return false;
        }

        return true;
    }
}