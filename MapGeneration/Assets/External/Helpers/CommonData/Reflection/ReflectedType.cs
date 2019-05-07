using System;
using System.Collections.Generic;
using System.Reflection;

//public class ReflectedType
//{
//    public Type Type;
//    public List<ReflectedMember> Members
//    {
//        get;
//        private set;
//    }

//    private bool? _hasDefaultConstructorCache;
//    private bool? _isDefaultConstructorPublicCache;

//    private ReflectedType(Type reflectedType)
//    {
//        Type = reflectedType;

//        Members = new List<ReflectedMember>();
//        CollectProperties();
//    }

//    private static void CollectProperties(List<ReflectedMember> reflectedMembers, Type reflectedType)
//    {
//        // do we require a [SerializeField] or [fsProperty] attribute?
//        //bool requireOptIn = config.DefaultMemberSerialization == fsMemberSerialization.OptIn;
//        //bool requireOptOut = config.DefaultMemberSerialization == fsMemberSerialization.OptOut;

//        //fsObjectAttribute attr = fsPortableReflection.GetAttribute<fsObjectAttribute>(reflectedType);
//        //if (attr != null)
//        //{
//        //    requireOptIn = attr.MemberSerialization == fsMemberSerialization.OptIn;
//        //    requireOptOut = attr.MemberSerialization == fsMemberSerialization.OptOut;
//        //}

//        MemberInfo[] members = Type.GetDeclaredMembers();
//        foreach (var member in members)
//        {
//            ReflectedMember reflectedMember = ReflectedMember.Get(member);
//            if (reflectedMember == null)
//                continue;
//            reflectedMembers.Add(reflectedMember);
           

//            // We don't serialize members annotated with any of the ignore
//            // serialize attributes
//            //if (config.IgnoreSerializeAttributes.Any(t => fsPortableReflection.HasAttribute(member, t)))
//            //{
//            //    continue;
//            //}
//            // Skip properties if we don't want them, to avoid the cost of
//            // checking attributes.
//            //if (property != null && !config.EnablePropertySerialization)
//            //{
//            //    continue;
//            //}
//            // If an opt-in annotation is required, then skip the property if
//            // it doesn't have one of the serialize attributes
//            //if (requireOptIn &&
//            //    !config.SerializeAttributes.Any(t => fsPortableReflection.HasAttribute(member, t)))
//            //{
//            //    continue;
//            //}
//            // If an opt-out annotation is required, then skip the property
//            // *only if* it has one of the not serialize attributes
//            //if (requireOptOut &&
//            //    config.IgnoreSerializeAttributes.Any(t => fsPortableReflection.HasAttribute(member, t)))
//            //{
//            //    continue;
//            //}
//        }

//        if (Type.Resolve().BaseType != null)
//        {
//            CollectProperties(config, properties, reflectedType.Resolve().BaseType);
//        }
//    }








//    /// <summary>
//    /// Attempt to emit an AOT compiled direct converter for this type.
//    /// </summary>
//    /// <returns>True if AOT data was emitted, false otherwise.</returns>
//    public void EmitAotData(bool throwException)
//    {
//        fsAotCompilationManager.AotCandidateTypes.Add(Type);
//        if (!throwException)
//            return;

//        // NOTE: Even if the type has derived types, we can still
//        // generate a direct converter for it. Direct converters are not
//        // used for inherited types, so the reflected converter or
//        // something similar will be used for the derived type instead of
//        // our AOT compiled one.

//        for (int i = 0; i < Members.Length; ++i)
//        {
//            // Cannot AOT compile since we need to public member access.
//            if (Members[i].IsPublic == false)
//                throw new AotFailureException(Type.CSharpName(true) + "::" + Members[i].MemberName + " is not public");
//            // Cannot AOT compile since readonly members can only be
//            // modified using reflection.
//            if (Members[i].IsReadOnly)
//                throw new AotFailureException(Type.CSharpName(true) + "::" + Members[i].MemberName + " is readonly");
//        }

//        // Cannot AOT compile since we need a default ctor.
//        if (HasDefaultConstructor == false)
//            throw new AotFailureException(ReflectedType.CSharpName(true) + " does not have a default constructor");
//    }

//    private static bool IsAutoProperty(PropertyInfo property)
//    {
//        return
//            property.CanWrite && property.CanRead &&
//            fsPortableReflection.HasAttribute(
//                    property.GetGetMethod(), typeof(CompilerGeneratedAttribute), /*shouldCache:*/false);
//    }
//}

//public class AotFailureException : Exception
//{
//    public AotFailureException(string reason) : base(reason) { }
//}