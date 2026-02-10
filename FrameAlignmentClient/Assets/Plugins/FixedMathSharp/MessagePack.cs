using System;

// 解决 [MessagePackObject(AllowPrivate = true)] 的报错
namespace MessagePack
{
    public class IgnoreMemberAttribute : Attribute { }
    public class KeyAttribute : Attribute
    {
        public KeyAttribute(int x) { }
    }
    public class MessagePackObjectAttribute : Attribute
    {
        // 补全参数，解决红波浪线
        public bool AllowPrivate { get; set; }
        public MessagePackObjectAttribute(bool allowPrivate = false)
        {
            this.AllowPrivate = allowPrivate;
        }
    }
    public class SerializationConstructorAttribute : Attribute { }
}

// 解决 Newtonsoft.Json 的报错
namespace Newtonsoft.Json
{
    public class JsonPropertyAttribute : Attribute
    {
        public JsonPropertyAttribute(string s) { }
    }
}

// 针对其他零散特性的补足
public class AllowPrivateAttribute : Attribute { }

// 针对 System.Drawing.Drawing2D 的报错
// 如果报错依然指向 using System.Drawing.Drawing2D，请双击报错手动删掉那行 using
namespace System.Drawing
{
    namespace Drawing2D
    {
        public class Dummy { }
    }
}