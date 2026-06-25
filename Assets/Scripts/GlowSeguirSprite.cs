using UnityEngine;

public class GlowSeguirSprite : MonoBehaviour
{
    public SpriteRenderer playerSprite;

    private SpriteRenderer glowSprite;

    void Awake()
    {
        glowSprite = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (playerSprite == null || glowSprite == null) return;

        glowSprite.sprite = playerSprite.sprite;
        glowSprite.flipX = playerSprite.flipX;
        glowSprite.flipY = playerSprite.flipY;
    }
}