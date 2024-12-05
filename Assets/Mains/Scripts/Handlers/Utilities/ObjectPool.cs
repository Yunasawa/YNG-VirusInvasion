using System.Collections.Generic;
using UnityEngine;
using System;
using YNL.Extensions.Methods;

namespace OWS.ObjectPooling
{
    public class ObjectPool<T> : IPool<T> where T : MonoBehaviour, IPoolable<T>
    {
        public ObjectPool(GameObject pooledObject, Transform container = null, int numToSpawn = 0)
        {
            this.prefab = pooledObject;
            this.container = container;
            Spawn(numToSpawn);
        }

        public ObjectPool(GameObject pooledObject, Action < T> pullObject, Action<T> pushObject, Transform container = null, int numToSpawn = 0)
        {
            this.prefab = pooledObject;
            this.container = container;
            this.pullObject = pullObject;
            this.pushObject = pushObject;
            Spawn(numToSpawn);
        }

        private System.Action<T> pullObject;
        private System.Action<T> pushObject;
        public Stack<T> pooledObjects = new Stack<T>();
        public Transform container;
        public List<T> activeObjects = new();
        private GameObject prefab;
        public int pooledCount
        {
            get
            {
                return pooledObjects.Count;
            }
        }

        public T Pull()
        {
            T t;
            if (pooledCount > 0) t = pooledObjects.Pop();
            else
            {
                if (container.IsNull()) t = GameObject.Instantiate(prefab).GetComponent<T>();
                else  t = GameObject.Instantiate(prefab, container).GetComponent<T>();
            }
            t.gameObject.SetActive(true); //ensure the object is on
            t.Initialize(Push);

            //allow default behavior and turning object back on
            pullObject?.Invoke(t);

            return t;
        }

        public T Pull(Vector3 position)
        {
            T t = Pull();
            t.transform.position = position;
            return t;
        }

        public T Pull(Vector3 position, Quaternion rotation)
        {
            T t = Pull();
            t.transform.position = position;
            t.transform.rotation = rotation;
            return t;
        }

        public GameObject PullGameObject()
        {
            return Pull().gameObject;
        }

        public GameObject PullGameObject(Vector3 position)
        {
            return PullGameObject(position, Quaternion.identity);
        }

        public GameObject PullGameObject(Vector3 position, Quaternion rotation)
        {
            T pullObject = Pull();
            activeObjects.Add(pullObject);
            GameObject go = pullObject.gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            return go;
        }

        public T PullObject(Vector3 position, Quaternion rotation)
        {
            T pullObject = Pull(position, rotation);
            activeObjects.Add(pullObject);
            return pullObject;
        }
        public T PullLocalObject(Vector3 position, Quaternion rotation)
        {
            T pullObject = Pull();
            activeObjects.Add(pullObject);
            pullObject.transform.localPosition = position;
            pullObject.transform.localRotation = rotation;
            return pullObject;
        }

        public void Push(T t)
        {
            pooledObjects.Push(t);
            activeObjects.Remove(t);

            pushObject?.Invoke(t);

            t.gameObject.SetActive(false);
        }

        private void Spawn(int number)
        {
            T t;

            for (int i = 0; i < number; i++)
            {
                if (container.IsNull()) t = GameObject.Instantiate(prefab).GetComponent<T>();
                else t = GameObject.Instantiate(prefab, container).GetComponent<T>();
                pooledObjects.Push(t);
                t.gameObject.SetActive(false);
            }
        }
    }

    public interface IPool<T>
    {
        T Pull();
        void Push(T t);
    }

    public interface IPoolable<T>
    {
        void Initialize(System.Action<T> returnAction);
        void ReturnToPool();
    }
}