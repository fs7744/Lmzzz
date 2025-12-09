//using System.Buffers;
//using System.Text;

//namespace Lmzzz.Bytes;

//public class IgnoreCaseBytesLiteral : ByteParser<ReadOnlySequence<byte>>
//{
//    private readonly int length;

//    private readonly Func<ReadOnlySequence<byte>, bool> func;

//    public IgnoreCaseBytesLiteral(string str, Encoding encoding)
//    {
//        this.length = encoding.GetByteCount(str);
//        this.func = BuildIgnoreCaseFunc(str, encoding);
//    }

//    public static Func<ReadOnlySequence<byte>, bool> BuildIgnoreCaseFunc(string str, Encoding encoding)
//    {
//        var l = str.ToLowerInvariant();
//        var u = str.ToUpperInvariant();
//        var length = encoding.GetByteCount(str);
//        var all = Enumerable.Range(0, length).Select(i =>
//        {
//            var ll = l[i];
//            var uu = u[i];
//            if (ll == uu)
//            {
//                return (encoding.GetBytes(ll.ToString()), null);
//            }
//            else
//            {
//                var bl = encoding.GetBytes(ll.ToString());
//                var bu = encoding.GetBytes(uu.ToString());
//                return (bl, bu);
//            }
//        }).ToArray();

//        return s =>
//        {
//            foreach (var item in all)
//            {
//                var (bl, bu) = item;
//                if (bu != null)
//                {
//                    if (s.StartsWith(bl) || s.StartsWith(bu))
//                    {
//                        s = s.Slice(bl.Length);
//                    }
//                    else
//                        return false;
//                }
//                else
//                {
//                    if (s.StartsWith(bl))
//                    {
//                        s = s.Slice(bl.Length);
//                    }
//                    else
//                        return false;
//                }
//            }
//            return true;
//        };


//        //Func<ReadOnlySequence<byte>, bool> f = s => true;

//        //foreach (var item in all.Reverse())
//        //{
//        //    var (len, ff) = item;
//        //    var bf = f;
//        //    f = s =>
//        //    {
//        //        if (ff(s))
//        //        {
//        //            return bf(s.Slice(len));
//        //        }

//        //        return false;
//        //    };
//        //}

//        //return f;
//    }

//    public override bool Parse(ref SequenceReader<byte> reader, out ByteParseResult<ReadOnlySequence<byte>> result)
//    {
//        if (!reader.End && reader.Remaining >= length)
//        {
//            if (func(reader.UnreadSequence))
//            {
//                var start = reader.Position;
//                reader.Advance(length);
//                result = new ByteParseResult<ReadOnlySequence<byte>>(start, reader.Position, reader.Sequence.Slice(start, reader.Position));
//                return true;
//            }
//        }

//        result = new ByteParseResult<ReadOnlySequence<byte>>();
//        return false;
//    }
//}
