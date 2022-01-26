using DotNetty.Buffers;

namespace fec;

public class InputOutputByteBufTableCodingLoop : ByteBufCodingLoopBase
{
    public void codeSomeShards(byte[][] matrixRows, IByteBuffer[] inputs, int inputCount, IByteBuffer[] outputs,
        int outputCount, int offset, int byteCount)
    {
        var table = Galois.MULTIPLICATION_TABLE;

        {
            var iInput = 0;
            var inputShard = inputs[iInput];
            for (var iOutput = 0; iOutput < outputCount; iOutput++)
            {
                var outputShard = outputs[iOutput];
                var matrixRow = matrixRows[iOutput];
                var multTableRow = table[matrixRow[iInput] & 0xFF];
                for (var iByte = offset; iByte < offset + byteCount; iByte++)
                    outputShard.SetByte(iByte, multTableRow[inputShard.GetByte(iByte) & 0xFF]);
                //outputShard[iByte] = multTableRow[inputShard[iByte] & 0xFF];
            }
        }

        for (var iInput = 1; iInput < inputCount; iInput++)
        {
            var inputShard = inputs[iInput];
            for (var iOutput = 0; iOutput < outputCount; iOutput++)
            {
                var outputShard = outputs[iOutput];
                var matrixRow = matrixRows[iOutput];
                var multTableRow = table[matrixRow[iInput] & 0xFF];
                for (var iByte = offset; iByte < offset + byteCount; iByte++)
                {
                    var temp = outputShard.GetByte(iByte);
                    temp ^= multTableRow[inputShard.GetByte(iByte) & 0xFF];
                    outputShard.SetByte(iByte, temp);
                    //outputShard[iByte] ^= multTableRow[inputShard[iByte] & 0xFF];
                }
            }
        }
    }


    public bool checkSomeShards(
        byte[][] matrixRows,
        IByteBuffer[] inputs, int inputCount,
        byte[][] toCheck, int checkCount,
        int offset, int byteCount,
        byte[] tempBuffer)
    {
        if (tempBuffer == null)
            return base.checkSomeShards(matrixRows, inputs, inputCount, toCheck, checkCount, offset, byteCount,
                null);

        // This is actually the code from OutputInputByteTableCodingLoop.
        // Using the loops from this class would require multiple temp
        // buffers.

        var table = Galois.MULTIPLICATION_TABLE;
        for (var iOutput = 0; iOutput < checkCount; iOutput++)
        {
            var outputShard = toCheck[iOutput];
            var matrixRow = matrixRows[iOutput];
            {
                var iInput = 0;
                var inputShard = inputs[iInput];
                var multTableRow = table[matrixRow[iInput] & 0xFF];
                for (var iByte = offset; iByte < offset + byteCount; iByte++)
                    tempBuffer[iByte] = multTableRow[inputShard.GetByte(iByte) & 0xFF];
            }
            for (var iInput = 1; iInput < inputCount; iInput++)
            {
                var inputShard = inputs[iInput];
                var multTableRow = table[matrixRow[iInput] & 0xFF];
                for (var iByte = offset; iByte < offset + byteCount; iByte++)
                    tempBuffer[iByte] ^= multTableRow[inputShard.GetByte(iByte) & 0xFF];
            }

            for (var iByte = offset; iByte < offset + byteCount; iByte++)
                if (tempBuffer[iByte] != outputShard[iByte])
                    return false;
        }

        return true;
    }
}