using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ControlCollider : MonoBehaviour
{

    public List<ColliderChild> Colliders = new List<ColliderChild>();
    private Collider2D[] hitResults = new Collider2D[10]; // Đệm dùng chung

    private void Update()
    {
        foreach (ColliderChild boxCheck in Colliders)
        {
            Vector2 offset = boxCheck.FolowScale
                ? new Vector2(boxCheck.Direction.x * Mathf.Sign(-transform.localScale.x), boxCheck.Direction.y)
                : -boxCheck.Direction;

            Vector2 boxCenter = (Vector2)transform.position + offset;

            int count = Physics2D.OverlapBoxNonAlloc(
                boxCenter,
                boxCheck.size,
                0f,
                hitResults,
                boxCheck.mask
            );

            bool isCol = false;

            for (int i = 0; i < count; i++)
            {
                Collider2D hit = hitResults[i];
                bool isVisible = true;

                if (boxCheck.CheckWall)
                {
                    RaycastHit2D obstacleHit = Physics2D.Linecast(
                        transform.position,
                        hit.transform.position,
                        LayerMask.GetMask("Map")
                    );
                    isVisible = obstacleHit.collider == null;
                }

                if (isVisible)
                {
                    isCol = true;
                    break;
                }
            }

            boxCheck.IsCol = isCol;
        }
    }

    private void OnDrawGizmos()
    {
        if (Colliders.Count == 0) return;
        foreach(ColliderChild boxCheck in Colliders)
        {

            if(boxCheck == null) continue;
            if (!boxCheck.Draw) continue;
            Gizmos.color = UnityEngine.Color.red;
            if (boxCheck.FolowScale)
            {
                float direction = Mathf.Sign(-transform.localScale.x); 

                Vector2 offset = new Vector2(boxCheck.Direction.x * direction, boxCheck.Direction.y);
                Vector2 boxCenter = (Vector2)transform.position + offset;

                Gizmos.DrawWireCube(boxCenter,
                    boxCheck.size);
            }
            else
            {
                Gizmos.DrawWireCube((Vector2)transform.position - boxCheck.Direction, boxCheck.size);
            }
        }
    }
    public ColliderChild GetColliderChild(string name)
    {
        ColliderChild r = new ColliderChild();
        foreach (ColliderChild boxCheck in Colliders)
        {
            if (boxCheck.Name == name)
            {
                r = boxCheck;
                break;
            }
        }
        return r;
    }
    public Collider2D GetCollider(string name)
    {
        Collider2D r = null;
        foreach (ColliderChild boxCheck in Colliders)
        {
            if (boxCheck.Name != name) continue;
            if (boxCheck.FolowScale)
            {
                float direction = Mathf.Sign(-transform.localScale.x); // +1 nếu phải, -1 nếu trái

                Vector2 offset = new Vector2(boxCheck.Direction.x * direction, boxCheck.Direction.y);
                Vector2 boxCenter = (Vector2)transform.position + offset;

                Collider2D hit = Physics2D.OverlapBox(
                    boxCenter,
                    boxCheck.size,
                    0f,
                    boxCheck.mask
                );
                if (hit != null)
                {
                    r = hit; break;
                }
                
            }
            else
            {
                Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position - boxCheck.Direction, boxCheck.size, 0f, boxCheck.mask);
                if (hit != null)
                {
                    r = hit; break;
                }
               
            }
        }
        return r;
    }
    public List<Collider2D> GetColliderForList(string name)
    {
        foreach (ColliderChild boxCheck in Colliders)
        {
            if (boxCheck.Name != name) continue;

            Vector2 boxCenter;
            if (boxCheck.FolowScale)
            {
                float direction = Mathf.Sign(-transform.localScale.x); // +1 nếu phải, -1 nếu trái
                Vector2 offset = new Vector2(boxCheck.Direction.x * direction, boxCheck.Direction.y);
                boxCenter = (Vector2)transform.position + offset;
            }
            else
            {
                boxCenter = (Vector2)transform.position - boxCheck.Direction;
            }

            Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxCheck.size, 0f, boxCheck.mask);
            return new List<Collider2D>(hits); // Trả về dù danh sách có thể rỗng
        }

        return new List<Collider2D>(); // Trả về danh sách rỗng nếu không khớp `name`
    }

    public Collider2D GetColliderAndMash(string name,string namemash)
    {
        Collider2D r = null;
        foreach (ColliderChild boxCheck in Colliders)
        {
            if (boxCheck.Name != name) continue;
            if (boxCheck.FolowScale)
            {
                float direction = Mathf.Sign(-transform.localScale.x); // +1 nếu phải, -1 nếu trái

                Vector2 offset = new Vector2(boxCheck.Direction.x * direction, boxCheck.Direction.y);
                Vector2 boxCenter = (Vector2)transform.position + offset;

                Collider2D hit = Physics2D.OverlapBox(
                    boxCenter,
                    boxCheck.size,
                    0f,
                    boxCheck.mask
                );
                if (hit != null)
                {
                    if (LayerMask.LayerToName(hit.gameObject.layer) != namemash) continue;
                    r = hit; break;
                }

            }
            else
            {
                Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position - boxCheck.Direction, boxCheck.size, 0f, boxCheck.mask);
                if (hit != null)
                {
                    if (LayerMask.LayerToName(hit.gameObject.layer) != namemash) continue;
                    r = hit; break;
                }

            }
        }
        return r;
    }
}
