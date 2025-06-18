using System.Collections;
using System.Text.Json.Serialization;

namespace ViennaDotNet.PreviewGenerator.NBT;

public class NbtList : IList
{
    public static readonly NbtList EMPTY = new NbtList(NbtType.END);

    [JsonInclude, JsonPropertyName("type")]
    public readonly NbtType _type;
    [JsonInclude, JsonPropertyName("array")]
    public readonly Array _array;
    [JsonIgnore]
    private bool hashCodeGenerated;
    [JsonIgnore]
    private int hashCode;

    public bool IsFixedSize => true;

    public bool IsReadOnly => true;

    public int Count => _array.Length;

    public bool IsSynchronized => false;

    public object SyncRoot => null!;

    public object? this[int index] { get => get(index); set => throw new InvalidOperationException(); }

    public NbtList(NbtType type, ICollection collection)
    {
        ArgumentNullException.ThrowIfNull(type, "tagClass");
        this._type = type;
        _array = Array.CreateInstance(type.getTagClass(), collection.Count);
        collection.CopyTo(_array, 0);
    }

    public NbtList(NbtType tagClass, params object[] array)
    {
        ArgumentNullException.ThrowIfNull(_type, "tagClass");
        _type = tagClass;
        this._array = (Array)array.Clone();
    }

    public NbtType getType()
    {
        return _type;
    }

    public object get(int index)
    {
        if (index < 0 || index >= _array.Length)
            throw new IndexOutOfRangeException("Expected 0-" + (_array.Length - 1) + ". Got " + index);

        return NbtUtils.copy(_array.GetValue(index)!);
    }

    public int size()
    {
        return _array.Length;
    }

    public int Add(object? value)
    {
        throw new InvalidOperationException();
    }

    public void Clear()
    {
        throw new InvalidOperationException();
    }

    public bool Contains(object? value)
    {
        return Array.IndexOf(_array, value) >= 0;
    }

    public int IndexOf(object? value)
    {
        return Array.IndexOf(_array, value);
    }

    public void Insert(int index, object? value)
    {
        throw new InvalidOperationException();
    }

    public void Remove(object? value)
    {
        throw new InvalidOperationException();
    }

    public void RemoveAt(int index)
    {
        throw new InvalidOperationException();
    }

    public void CopyTo(Array array, int index)
    {
        this._array.CopyTo(array, index);
    }

    public IEnumerator GetEnumerator()
    {
        return _array.GetEnumerator();
    }
}
