using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voodoo.Render
{
	[Serializable]
	internal class SpherePropertyList : IList<SphereProperty>
	{
		[SerializeField] private List<SphereProperty> properties = new List<SphereProperty>();
		
		public SphereProperty this[int index] { get => properties[index]; set => properties[index] = value; }
		
		public int Count => properties.Count;
		public bool IsReadOnly => false;
		
        public void Add(SphereProperty item)
        {
	        if (item == null)
	        {
		        return;
	        }
	        
	        SphereProperty sp = properties.Find(x => x.matcapId == item.matcapId);
        	if (sp != null)
            {
	            properties[properties.IndexOf(sp)] = item;
        		return;
        	}
        	
        	properties.Add(item);
        }
        
        public bool Remove(SphereProperty item)
        {
	        if (item == null || properties.Exists(x => x.matcapId == item.matcapId) == false)
	        {
		        return false;
	        }
        	
	        return properties.Remove(item);
        }

        public SphereProperty Find(Predicate<SphereProperty> condition)
        {
	        return properties.Find(condition);
        }

        public void Clear()
        {
	        properties.Clear();
        }

        public bool Contains(SphereProperty item)
        {
	        return properties.Contains(item);
        }

        public void CopyTo(SphereProperty[] array, int arrayIndex)
        {
	        properties.CopyTo(array, arrayIndex);
        }

        public IEnumerator<SphereProperty> GetEnumerator()
        {
	        return properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
	        return GetEnumerator();
        }

        public int IndexOf(SphereProperty item)
        {
	        return properties.IndexOf(item);
        }

        public void Insert(int index, SphereProperty item)
        {
	        properties.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
	        properties.RemoveAt(index);
        }
	}
}