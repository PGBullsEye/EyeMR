// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DestroyChildren.cs" company="BullsEye">
//  Author: Aljoscha Niazi-Shahabi
// </copyright>
// <summary>
// The GoogleVR Viewer finds all cameras in the scene and attaches two more cameras as child objects to render the 
// split screen. This Script, when attached to a certain Camera, destroys the child objects of that very camera.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BullsEye.Scripts.AR
{
    using UnityEngine;

    /// <summary>
    /// This Script, when attached to a certain Camera, destroys the child objects of that very camera.
    /// </summary>
    public class DestroyChildren : MonoBehaviour
    {
        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public void Update()
        {
            if (transform.childCount > 0)
            {
                this.DeactivateAllChildren();
            }
        }

        /// <summary>
        /// This method destroys all gameObjects attached to this script's gameObject.
        /// </summary>
        private void DeactivateAllChildren()
        {
            foreach (Transform child in this.transform)
            {
                // child.gameObject.SetActive(false);
                MonoBehaviour.DestroyObject(child.gameObject);
            }       
        } 
    }
}
