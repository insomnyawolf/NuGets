//using System;
//using System.Collections.Generic;
//using System.Reflection.Emit;
//using System.Reflection;

//namespace Extensions.Reflection
//{
//    // What the heck??
//    public static class DeepCloneIlEmitClass
//    {
//        public delegate dynamic DeepCloneDelegate(dynamic input);
//        public static Type DeepCloneDelegateCachedType = typeof(DeepCloneDelegate);

//        static Dictionary<Type, DeepCloneDelegate> CachedIl = new Dictionary<Type, DeepCloneDelegate>();

//        private static T CloneObjectWithIL<T>(this T myObject)
//        {
//            var cachedType = typeof(T);

//            if (CachedIl.TryGetValue(cachedType, out DeepCloneDelegate myExec))
//            {
//                return (T)myExec(myObject);
//            }

//            // Create ILGenerator
//            DynamicMethod dymMethod = new DynamicMethod("DoClone", cachedType, new Type[] { cachedType }, true);

//            ConstructorInfo cInfo = cachedType.GetConstructor(new Type[] { });

//            ILGenerator generator = dymMethod.GetILGenerator();

//            LocalBuilder lbf = generator.DeclareLocal(cachedType);
//            //lbf.SetLocalSymInfo("_temp");

//            generator.Emit(OpCodes.Newobj, cInfo);
//            generator.Emit(OpCodes.Stloc_0);

//            foreach (FieldInfo field in cachedType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
//            {
//                // Load the new object on the eval stack... (currently 1 item on eval stack)
//                generator.Emit(OpCodes.Ldloc_0);
//                // Load initial object (parameter)          (currently 2 items on eval stack)
//                generator.Emit(OpCodes.Ldarg_0);
//                // Replace value by field value             (still currently 2 items on eval stack)
//                generator.Emit(OpCodes.Ldfld, field);
//                // Store the value of the top on the eval stack into the object underneath that value on the value stack.
//                //  (0 items on eval stack)
//                generator.Emit(OpCodes.Stfld, field);
//            }

//            // Load new constructed obj on eval stack -> 1 item on stack
//            generator.Emit(OpCodes.Ldloc_0);
//            // Return constructed object.   --> 0 items on stack
//            generator.Emit(OpCodes.Ret);

//            myExec = (DeepCloneDelegate)dymMethod.CreateDelegate(DeepCloneDelegateCachedType);

//            CachedIl.Add(cachedType, myExec);

//            return (T)myExec(myObject);
//        }
//    }
//}
