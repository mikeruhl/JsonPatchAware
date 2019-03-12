using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Newtonsoft.Json.Serialization;

namespace JsonPatchAware
{
    public class AwareAdapterFactory: AdapterFactory
    {
        public override IAdapter Create(object target, IContractResolver contractResolver)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (contractResolver == null)
            {
                throw new ArgumentNullException(nameof(contractResolver));
            }

            var jsonContract = contractResolver.ResolveContract(target.GetType());

            if (target is IList)
            {
                return new ListAdapter();
            }
            else if (jsonContract is JsonDictionaryContract jsonDictionaryContract)
            {
                var type = typeof(DictionaryAdapter<,>).MakeGenericType(jsonDictionaryContract.DictionaryKeyType, jsonDictionaryContract.DictionaryValueType);
                return (IAdapter)Activator.CreateInstance(type);
            }
            else if (jsonContract is JsonDynamicContract)
            {
                return new DynamicObjectAdapter();
            }
            else
            {
                return new AwarePocoAdapter();
            }
        }
    }
}
