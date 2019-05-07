using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace helper
{
    public static class HelperUnity
    {
        public static GameObject findGameObject(string name)
        {
            GameObject go = GameObject.Find(name);

            if (go == null)
                Debug.Log("GameObject by the name: " + name + " cannot be found!");

            return GameObject.Find("name");
        }

        public static GameObject findChildGO(string childName, GameObject parent)
        {
            if (parent == null)
            {
                Debug.Log("Parent is null!Cannot look for children");
                return null;
            }

            Transform child = parent.transform.Find(childName);
            if (child == null)
            {
                Debug.Log("No child of name: " + childName + " exists on gameObject: " + parent);
                return null;
            }
            return child.gameObject;
        }

        public static T findComponentOnChild<T>(string childName, GameObject parent) where T : Component
        {
            GameObject childGO = findChildGO(childName, parent);

            if (childGO != null)
                return childGO.GetComponent<T>();
            return null;
        }

        public static T findComponentOnGameObject<T>(string goName) where T : Component
        {
            GameObject go = GameObject.Find(goName);
            if (go != null)
            {
                T comp = go.GetComponent<T>();

                if (comp == null)
                    Debug.Log("Component of type" + typeof(T).ToString() + " doesnt exist on Gameobject: " + goName);
                return comp;
            }

            return null;
        }

        public static T SpawnGO<T>(T prefab, Vector3 pos, Transform parent) where T : MonoBehaviour //TODO: prolly move this method out of this class
        {
            T newGO = GameObject.Instantiate(prefab, pos, Quaternion.identity).GetComponent<T>();
            newGO.transform.SetParent(parent);

            return newGO;
        }

        public static void GetChildrenTransforms(Transform t, List<Transform> childComponents, bool doRecursive = false)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                childComponents.Add(child);

                if (doRecursive)
                    GetChildrenTransforms(child, childComponents, doRecursive);
            }
        }

        public static void GetAllChildrenComponents<T>(Transform t, List<T> childComponents, bool doRecursive = true) where T : Component
        {
            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);

                T c = child.GetComponent<T>();
                if (c != null)
                    childComponents.Add(c);

                if (doRecursive)
                    GetAllChildrenComponents<T>(child, childComponents, doRecursive);
            }
        }

        public static void GetAllChildrenComponents<T>(IList<Transform> gos, List<T> components, bool doRecursive = true) where T : Component
        {
            foreach (var go in gos)
            {
                GetAllChildrenComponents<T>(go, components, doRecursive);
            }
        }

        public static void SetLayerToAllChildren(GameObject parent, int layer)
        {
            parent.layer = layer;
            for (int i = 0; i < parent.transform.childCount; i++)
                HelperUnity.SetLayerToAllChildren(parent.transform.GetChild(i).gameObject, layer);
        }

        public static void DestroyAllChildren(Transform t)
        {
            while (t.childCount > 0)
            {
                Transform childT = t.GetChild(t.childCount - 1);

                childT.SetParent(null);

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    GameObject.DestroyImmediate(childT.gameObject);
                else
                    GameObject.Destroy(childT.gameObject);
#else
            GameObject.Destroy(childT.gameObject);
#endif
            }
        }

        /// <summary>
        /// Exits the application in approptiate way
        /// </summary>
        public static void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }


        public static void DestroyAllChildrenOfType<T>(Transform t, bool isRecursive) where T : Component
        {
            T[] allComponents = (isRecursive) ? t.GetComponentsInChildren<T>() : t.GetComponents<T>();

            foreach (T component in allComponents)
            {
                if (component != null && component.transform.parent != null)
                {
                    component.transform.SetParent(null);

#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        GameObject.DestroyImmediate(component.gameObject);
                    else
                        GameObject.Destroy(component.gameObject);
#else
                GameObject.Destroy(component.gameObject);
#endif
                }
            }
        }

        //TODO: only returns one if multi touched
        public static List<Vector2> GetClickedOrTouchedPositions()
        {
            List<Vector2> positions = new List<Vector2>();
            if (Input.GetMouseButtonDown(0))
            {
                positions.Add(Input.mousePosition);
            }
            else if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                    positions.Add(Input.GetTouch(0).position);
            }
            return positions;
        }

        public static Vector3 getMouseWorldPos(Camera camera)
        {
            return camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}


