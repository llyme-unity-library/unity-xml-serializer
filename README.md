# unity-xml-serializer
Transforms this

<img src="https://raw.githubusercontent.com/llyme-unity-library/unity-xml-serializer/main/Sample0.png" width="256">

Into this

<img src="https://raw.githubusercontent.com/llyme-unity-library/unity-xml-serializer/main/Sample1.png" width="256">

And back

<img src="https://raw.githubusercontent.com/llyme-unity-library/unity-xml-serializer/main/Sample0.png" width="256">

# Overview
Transforms unity `Objects` into `Xml`, including `Components`. Supports circular referencing.

# Requirements
* [XmlHelper](https://github.com/llyme-unity-library/unity-xml-helper)
* [TypeHelper](https://github.com/llyme-unity-library/unity-type-helper)
* [TextHelper](https://github.com/llyme-unity-library/unity-text-helper)

# How to Use
Inherit the `Serialize` and `Deserialize` classes.

It is adviced that the `Resolver` virtual functions be overridden to fully
utilize the serializer, as some objects (such as `Materials`, `Textures`, etc.)
may not be serialized and deserialized properly.

```csharp
public class Serialize : UnityXmlSerializer.Serialize<Serialize>
{
  protected override bool StringifyResolver(object value, out string result)
  {
    // You can do this to change how the serializer behaves.
    // ...
  }

  // There are more resolvers to override.

  // ...
}

public class Deserialize : UnityXmlSerializer.Deserialize<Deserialize>
{
  // There are more resolvers to override here as well.
  
  // ...
}
```

Then call them with an `XmlWriter`.

```csharp
Serialize serialize = new Serialize();
serialize.Set(@object: /*GameObject*/, writer: /*XmlWriter*/);
serialize.Do();
// Done!
```

```csharp
Deserialize deserialize = new Deserialize();
deserialize.Set(document: /*XmlDocument*/, parent: /*GameObject.transform*/);

IEnumerator Do()
{
  yield return deserialize.Do();

  Debug.Log($"Deserialized object '{deserialize.Value}'.");
}

// Loop through the enumerator!
StartCoroutine(Do());
```

# Tips

You can use Unity's `[SerializeField]` attribute to forcefully serialize non-literal fields.

```csharp
public class IWantToBeSerialized
{
  private int i_will_not_be_serialized = 42;
  [SerializeField]
  private string i_will_be_serialized = "yes i will";
  public bool i_will_also_be_serialized = true;

  [field: SerializeField]
  public string Properties_As_Well { get; set; } = "nice";
}
```

You can use `[IgnoreParentValue]` attribute to create an entirely new instance instead of using what's already inside the object.
This is useful when you are dealing with arrays and dictionaries.

```csharp
public class SomeClass
{
  [IgnoreParentValue]
  public List<string> = new() // When deserialized, the `new()` will be overridden.
  {
    "Hello", // You won't see this one from a deserialized object.
    "World" // You won't see this either.
  };
}
```
