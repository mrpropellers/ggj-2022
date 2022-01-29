using UnityEngine;

namespace GGJ.Utility.Runtime.Utility
{
    public static class CameraHelpers
    {
        public static void ResizeSpriteToFitScreen(Camera camera, SpriteRenderer renderer)
        {
            var worldScreenHeight = camera.orthographicSize * 2;
            if (Screen.height == 0 || Screen.width == 0)
            {
                // This is not an error case, we're just running in an inactive editor scene
                return;
            }
            var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            var sprite = renderer.sprite;
            renderer.transform.position = Vector3.forward;
            renderer.transform.localScale = new Vector3(
                worldScreenWidth / sprite.bounds.size.x,
                worldScreenHeight / sprite.bounds.size.y, 1);
        }

    }
}
