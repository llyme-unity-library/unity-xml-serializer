# unity-xml-serializer
Transforms this

<img src="https://raw.githubusercontent.com/llyme-unity-library/unity-xml-serializer/main/Sample0.png" width="256">

Into this

<img src="https://raw.githubusercontent.com/llyme-unity-library/unity-xml-serializer/main/Sample1.png" width="256">

And back

<img src="https://raw.githubusercontent.com/llyme-unity-library/unity-xml-serializer/main/Sample0.png" width="256">

# Overview
Transforms unity `Objects` into `Xml`, including `Components`. Supports circular referencing.

# How to Use
Inherit the `Serialize` and `Deserialize` classes.

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
