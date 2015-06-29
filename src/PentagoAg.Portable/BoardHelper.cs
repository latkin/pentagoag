using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PentagoAgEngine
{
    // Helper class for all the bitboard stuff
    class BoardHelper
    {
        public static UInt64 Zeros = 0x0000000000000000;
        public static UInt64 Ones = 0xFFFFFFFFFFFFFFFF;
        public static UInt64 LeftOne = 0x8000000000000000;
        public static UInt64[] Bits = {
                                        0x8000000000000000,
                                        0x4000000000000000,
                                        0x2000000000000000,
                                        0x1000000000000000,
                                        0x0800000000000000,
                                        0x0400000000000000,
                                        0x0200000000000000,
                                        0x0100000000000000,
                                        0x0080000000000000,
                                        0x0040000000000000,
                                        0x0020000000000000,
                                        0x0010000000000000,
                                        0x0008000000000000,
                                        0x0004000000000000,
                                        0x0002000000000000,
                                        0x0001000000000000,
                                        0x0000800000000000,
                                        0x0000400000000000,
                                        0x0000200000000000,
                                        0x0000100000000000,
                                        0x0000080000000000,
                                        0x0000040000000000,
                                        0x0000020000000000,
                                        0x0000010000000000,
                                        0x0000008000000000,
                                        0x0000004000000000,
                                        0x0000002000000000,
                                        0x0000001000000000,
                                        0x0000000800000000,
                                        0x0000000400000000,
                                        0x0000000200000000,
                                        0x0000000100000000,
                                        0x0000000080000000,
                                        0x0000000040000000,
                                        0x0000000020000000,
                                        0x0000000010000000,
                                        0x0000000008000000,
                                        0x0000000004000000,
                                        0x0000000002000000,
                                        0x0000000001000000,
                                        0x0000000000800000,
                                        0x0000000000400000,
                                        0x0000000000200000,
                                        0x0000000000100000,
                                        0x0000000000080000,
                                        0x0000000000040000,
                                        0x0000000000020000,
                                        0x0000000000010000,
                                        0x0000000000008000,
                                        0x0000000000004000,
                                        0x0000000000002000,
                                        0x0000000000001000,
                                        0x0000000000000800,
                                        0x0000000000000400,
                                        0x0000000000000200,
                                        0x0000000000000100,
                                        0x0000000000000080,
                                        0x0000000000000040,
                                        0x0000000000000020,
                                        0x0000000000000010,
                                        0x0000000000000008,
                                        0x0000000000000004,
                                        0x0000000000000002,
                                        0x0000000000000001
                                };
        public static UInt64[] EmptyBits = {
                                        0x7FFFFFFFFFFFFFFF,
                                        0xBFFFFFFFFFFFFFFF,
                                        0xDFFFFFFFFFFFFFFF,
                                        0xEFFFFFFFFFFFFFFF,
                                        0xF7FFFFFFFFFFFFFF,
                                        0xFBFFFFFFFFFFFFFF,
                                        0xFDFFFFFFFFFFFFFF,
                                        0xFEFFFFFFFFFFFFFF,
                                        0xFF7FFFFFFFFFFFFF,
                                        0xFFBFFFFFFFFFFFFF,
                                        0xFFDFFFFFFFFFFFFF,
                                        0xFFEFFFFFFFFFFFFF,
                                        0xFFF7FFFFFFFFFFFF,
                                        0xFFFBFFFFFFFFFFFF,
                                        0xFFFDFFFFFFFFFFFF,
                                        0xFFFEFFFFFFFFFFFF,
                                        0xFFFF7FFFFFFFFFFF,
                                        0xFFFFBFFFFFFFFFFF,
                                        0xFFFFDFFFFFFFFFFF,
                                        0xFFFFEFFFFFFFFFFF,
                                        0xFFFFF7FFFFFFFFFF,
                                        0xFFFFFBFFFFFFFFFF,
                                        0xFFFFFDFFFFFFFFFF,
                                        0xFFFFFEFFFFFFFFFF,
                                        0xFFFFFF7FFFFFFFFF,
                                        0xFFFFFFBFFFFFFFFF,
                                        0xFFFFFFDFFFFFFFFF,
                                        0xFFFFFFEFFFFFFFFF,
                                        0xFFFFFFF7FFFFFFFF,
                                        0xFFFFFFFBFFFFFFFF,
                                        0xFFFFFFFDFFFFFFFF,
                                        0xFFFFFFFEFFFFFFFF,
                                        0xFFFFFFFF7FFFFFFF,
                                        0xFFFFFFFFBFFFFFFF,
                                        0xFFFFFFFFDFFFFFFF,
                                        0xFFFFFFFFEFFFFFFF,
                                        0xFFFFFFFFF7FFFFFF,
                                        0xFFFFFFFFFBFFFFFF,
                                        0xFFFFFFFFFDFFFFFF,
                                        0xFFFFFFFFFEFFFFFF,
                                        0xFFFFFFFFFF7FFFFF,
                                        0xFFFFFFFFFFBFFFFF,
                                        0xFFFFFFFFFFDFFFFF,
                                        0xFFFFFFFFFFEFFFFF,
                                        0xFFFFFFFFFFF7FFFF,
                                        0xFFFFFFFFFFFBFFFF,
                                        0xFFFFFFFFFFFDFFFF,
                                        0xFFFFFFFFFFFEFFFF,
                                        0xFFFFFFFFFFFF7FFF,
                                        0xFFFFFFFFFFFFBFFF,
                                        0xFFFFFFFFFFFFDFFF,
                                        0xFFFFFFFFFFFFEFFF,
                                        0xFFFFFFFFFFFFF7FF,
                                        0xFFFFFFFFFFFFFBFF,
                                        0xFFFFFFFFFFFFFDFF,
                                        0xFFFFFFFFFFFFFEFF,
                                        0xFFFFFFFFFFFFFF7F,
                                        0xFFFFFFFFFFFFFFBF,
                                        0xFFFFFFFFFFFFFFDF,
                                        0xFFFFFFFFFFFFFFEF,
                                        0xFFFFFFFFFFFFFFF7,
                                        0xFFFFFFFFFFFFFFFB,
                                        0xFFFFFFFFFFFFFFFD,
                                        0xFFFFFFFFFFFFFFFE
                                  };
        public static UInt64[] WinningLines = {
                                           // vertical
                                            0x8208208000000000, // 100000100000100000100000100000000000
                                            0x4104104000000000, // 010000010000010000010000010000000000
                                            0x2082082000000000, // 001000001000001000001000001000000000
                                            0x1041041000000000, // 000100000100000100000100000100000000
                                            0x0820820800000000, // 000010000010000010000010000010000000
                                            0x0410410400000000, // 000001000001000001000001000001000000
                                            0x0208208200000000, // 000000100000100000100000100000100000
                                            0x0104104100000000, // 000000010000010000010000010000010000
                                            0x0082082080000000, // 000000001000001000001000001000001000
                                            0x0041041040000000, // 000000000100000100000100000100000100
                                            0x0020820820000000, // 000000000010000010000010000010000010
                                            0x0010410410000000, // 000000000001000001000001000001000001
                                            // horizontal
                                            0xF800000000000000, // 111110000000000000000000000000000000
                                            0x03E0000000000000, // 000000111110000000000000000000000000
                                            0x000F800000000000, // 000000000000111110000000000000000000
                                            0x00003E0000000000, // 000000000000000000111110000000000000
                                            0x000000F800000000, // 000000000000000000000000111110000000
                                            0x00000003E0000000, // 000000000000000000000000000000111110
                                            0x7C00000000000000, // 011111000000000000000000000000000000
                                            0x01F0000000000000, // 000000011111000000000000000000000000
                                            0x0007C00000000000, // 000000000000011111000000000000000000
                                            0x00001F0000000000, // 000000000000000000011111000000000000
                                            0x0000007C00000000, // 000000000000000000000000011111000000
                                            0x00000001F0000000, // 000000000000000000000000000000011111
                                            // main diagonals
                                            0x8102040800000000, // 100000010000001000000100000010000000
                                            0x0102040810000000, // 000000010000001000000100000010000001
                                            0x0021084200000000, // 000000000010000100001000010000100000
                                            0x0421084000000000, // 000001000010000100001000010000000000
                                            // offset diagonals
                                            0x4081020400000000, // 010000001000000100000010000001000000
                                            0x0204081020000000, // 000000100000010000001000000100000010
                                            0x0842108000000000, // 000010000100001000010000100000000000
                                            0x0010842100000000, // 000000000001000010000100001000010000
                                       };

        public static int[] UpperLeftClockwiseIndices = { 18, 20, 32, 30, 24, 19, 26, 31 };
        public static int[] UpperLeftAntiClockwiseIndices = { 18, 30, 32, 20, 24, 31, 26, 19 };
        public static int[] UpperRightClockwiseIndices = { 21, 23, 35, 33, 27, 22, 29, 34 };
        public static int[] UpperRightAntiClockwiseIndices = { 21, 33, 35, 23, 27, 34, 29, 22 };
        public static int[] LowerLeftClockwiseIndices = { 0, 2, 14, 12, 6, 1, 8, 13 };
        public static int[] LowerLeftAntiClockwiseIndices = { 0, 12, 14, 2, 6, 13, 8, 1 };
        public static int[] LowerRightClockwiseIndices = { 3, 5, 17, 15, 9, 4, 11, 16 };
        public static int[] LowerRightAntiClockwiseIndices = { 3, 15, 17, 5, 9, 16, 11, 4 };

        public static int NumOneBits(UInt64 num)
        {
            UInt64 copy = num;
            int result = 0;
            do
            {
                if ((copy & LeftOne) != 0)
                    result++;
                copy = copy << 1;
            } while (copy != 0);
            return result;
        }

        public static int NumCommonOneBits(UInt64 left, UInt64 right)
        {
            return NumOneBits(left & right);
        }
    }
}
