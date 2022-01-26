/**
 * One specific ordering/nesting of the coding loops.
 *
 * Copyright 2015, Backblaze, Inc.  All rights reserved.
 */

namespace fec;

public class OutputInputByteTableCodingLoop : CodingLoopBase
{
    public override void codeSomeShards(
        byte[][] matrixRows,
        byte[][] inputs, int inputCount,
        byte[][] outputs, int outputCount,
        int offset, int byteCount)
    {
        var table = Galois.MULTIPLICATION_TABLE;
        for (var iOutput = 0; iOutput < outputCount; iOutput++)
        {
            var outputShard = outputs[iOutput];
            var matrixRow = matrixRows[iOutput];
            {
                var iInput = 0;
                var inputShard = inputs[iInput];
                var multTableRow = table[matrixRow[iInput]];
                for (var iByte = offset; iByte < offset + byteCount; iByte++)
                    outputShard[iByte] = multTableRow[inputShard[iByte]];
            }
            for (var iInput = 1; iInput < inputCount; iInput++)
            {
                var inputShard = inputs[iInput];
                var multTableRow = table[matrixRow[iInput] & 0xFF];
                for (var iByte = offset; iByte < offset + byteCount; iByte++)
                    outputShard[iByte] ^= multTableRow[inputShard[iByte]];
            }
        }
    }


    public override bool checkSomeShards(
        byte[][] matrixRows,
        byte[][] inputs, int inputCount,
        byte[][] toCheck, int checkCount,
        int offset, int byteCount,
        byte[] tempBuffer)
    {
        if (tempBuffer == null)
            return base.checkSomeShards(matrixRows, inputs, inputCount, toCheck, checkCount, offset, byteCount,
                null);

        var table = Galois.MULTIPLICATION_TABLE;
        for (var iOutput = 0; iOutput < checkCount; iOutput++)
        {
            var outputShard = toCheck[iOutput];
            var matrixRow = matrixRows[iOutput];
            {
                var iInput = 0;
                var inputShard = inputs[iInput];
                var multTableRow = table[matrixRow[iInput]];
                for (var iByte = offset; iByte < offset + byteCount; iByte++)
                    tempBuffer[iByte] = multTableRow[inputShard[iByte]];
            }
            for (var iInput = 1; iInput < inputCount; iInput++)
            {
                var inputShard = inputs[iInput];
                var multTableRow = table[matrixRow[iInput]];
                for (var iByte = offset; iByte < offset + byteCount; iByte++)
                    tempBuffer[iByte] ^= multTableRow[inputShard[iByte]];
            }

            for (var iByte = offset; iByte < offset + byteCount; iByte++)
                if (tempBuffer[iByte] != outputShard[iByte])
                    return false;
        }

        return true;
    }
}