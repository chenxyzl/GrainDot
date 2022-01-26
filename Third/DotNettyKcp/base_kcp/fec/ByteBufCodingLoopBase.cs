using DotNetty.Buffers;

namespace fec;

public class ByteBufCodingLoopBase : ByteBufCodingLoop
{
    public virtual void codeSomeShards(byte[][] matrixRows, IByteBuffer[] inputs, int inputCount,
        IByteBuffer[] outputs, int outputCount,
        int offset, int byteCount)
    {
    }

    public virtual bool checkSomeShards(byte[][] matrixRows, IByteBuffer[] inputs, int inputCount, byte[][] toCheck,
        int checkCount,
        int offset, int byteCount, byte[] tempBuffer)
    {
        var table = Galois.MULTIPLICATION_TABLE;
        for (var iByte = offset; iByte < offset + byteCount; iByte++)
        for (var iOutput = 0; iOutput < checkCount; iOutput++)
        {
            var matrixRow = matrixRows[iOutput];
            var value = 0;
            for (var iInput = 0; iInput < inputCount; iInput++)
                value ^= table[matrixRow[iInput] & 0xFF][inputs[iInput].GetByte(iByte) & 0xFF];

            if (toCheck[iOutput][iByte] != (byte) value) return false;
        }

        return true;
    }
}