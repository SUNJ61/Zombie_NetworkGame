using ExitGames.Client.Photon;
using UnityEngine;
//컬러의 4개의 변수를 직렬화, 역직렬화 하는 코드 (직렬화 : 코드상의 변수를 byte로 변경해 통신 가능하도록 바꾸는 것, 역직렬화 : 통신으로 받은 byte를 코드상의 변수로 바꾸는것)
public class ColorSerialization {
    private static byte[] colorMemory = new byte[4 * 4];

    public static short SerializeColor(StreamBuffer outStream, object targetObject) {
        Color color = (Color) targetObject;

        lock (colorMemory)
        {
            byte[] bytes = colorMemory;
            int index = 0;
            
            Protocol.Serialize(color.r, bytes, ref index);
            Protocol.Serialize(color.g, bytes, ref index);
            Protocol.Serialize(color.b, bytes, ref index);
            Protocol.Serialize(color.a, bytes, ref index);
            outStream.Write(bytes, 0, 4*4);
        }

        return 4 * 4;
    }

    public static object DeserializeColor(StreamBuffer inStream, short length)  {
        Color color = new Color();
  
        lock (colorMemory)
        {
            inStream.Read(colorMemory, 0, 4 * 4);
            int index = 0;
            
            Protocol.Deserialize(out color.r,colorMemory, ref index);
            Protocol.Deserialize(out color.g,colorMemory, ref index);
            Protocol.Deserialize(out color.b,colorMemory, ref index);
            Protocol.Deserialize(out color.a,colorMemory, ref index);
        }
        
        return color;
    }
}