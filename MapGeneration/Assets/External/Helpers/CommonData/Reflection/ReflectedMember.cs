using System;
using System.Reflection;

public class ReflectedMember
{
    private MemberInfo m_member;
    bool m_isProperty = false;
    bool m_isField = false;

    //do this with memberInfo
    private ReflectedMember(MemberInfo member)
    {
        m_member = member;
        if (member is FieldInfo)
            m_isField = true;
        else if (member is PropertyInfo)
            m_isProperty = true;


        //_memberInfo = field;
        //StorageType = field.FieldType;
        //MemberName = field.Name;
        //IsPublic = field.IsPublic;
        //IsReadOnly = field.IsInitOnly;
        //CanRead = true;
        //CanWrite = true;
    }
    //private ReflectedMember(PropertyInfo property)
    //{
    //    m_member = property;
    //    StorageType = property.PropertyType;
    //    MemberName = property.Name;
    //    IsPublic = (property.GetGetMethod() != null && property.GetGetMethod().IsPublic) &&
    //               (property.GetSetMethod() != null && property.GetSetMethod().IsPublic);
    //    IsReadOnly = false;
    //    CanRead = property.CanRead;
    //    CanWrite = property.CanWrite;
    //}



    //*************
    // FUNCTIONALITY
    public bool IsValid
    {
        get
        {
            return m_member != null && (m_isField || m_isProperty);
    }
    }

    /// <summary>
    /// Reads a value from the property that this MetaProperty represents,
    /// using the given object instance as the context.
    /// </summary>
    public object Read(object context)
    {
        if (m_isProperty)
        {
            return ((PropertyInfo)m_member).GetValue(context, new object[] { });
        }
        else
        {
            return ((FieldInfo)m_member).GetValue(context);
        }
    }

    /// <summary>
    /// Writes a value to the property that this MetaProperty represents,
    /// using given object instance as the context.
    /// </summary>
    public void Write(object context, object value)
    {
        FieldInfo field = m_member as FieldInfo;
        PropertyInfo property = m_member as PropertyInfo;

        if (field != null)
        {
            field.SetValue(context, value);
        }
        else if (property != null)
        {
            MethodInfo setMethod = property.GetSetMethod(true);
            if (setMethod != null)
            {
                setMethod.Invoke(context, new object[] { value });
            }
        }
    }

    //***************
    // PROPERTIES

    /// <summary>
    /// The type of value that is stored inside of the property. For example,
    /// for an int field, StorageType will be typeof(int).
    /// </summary>
    public Type StorageType
    {
        get;
        private set;
    }

    /// <summary>
    /// Is this type readonly? We can modify readonly properties using
    /// reflection, but not using generated C#.
    /// </summary>
    public bool IsReadOnly
    {
        get; private set;
    }

    /// <summary>
    /// Is this member public?
    /// </summary>
    public bool IsPublic
    {
        get;
        private set;
    }

    /// <summary>
    /// Can this property be read?
    /// </summary>
    public bool CanRead
    {
        get;
        private set;
    }

    /// <summary>
    /// Can this property be written to?
    /// </summary>
    public bool CanWrite
    {
        get;
        private set;
    }

    /// <summary>
    /// The name of the actual member.
    /// </summary>
    public string MemberName
    {
        get;
        private set;
    }
}
