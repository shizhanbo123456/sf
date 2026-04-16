using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    /// <summary>
    /// 组装后初始化，初始化无依赖
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(BulletDetector))]
    [RequireComponent(typeof(GroundDetector))]
    public class TargetGraphic : MonoBehaviour
    {
        public const string NullName = "null";

        public float SpawnOffset
        {
            get
            {
                if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
                return boxCollider.edgeRadius - (boxCollider.offset.y - boxCollider.size.y / 2f) * transform.localScale.y;
            }
        }

        [SerializeField] private bool SetState = false;
        private static readonly Vector3 R = new Vector3(1, 1, 1);
        private static readonly Vector3 L = new Vector3(-1, 1, 1);

        private Animator anim;
        private ITargetcontrollerInfo Icontroller;
        private Rigidbody2D rb;

        private SpriteRenderer minimapIcon;

        [HideInInspector] public BulletDetector bulletDetector;
        [HideInInspector] public GroundDetector groundDetector;
        [HideInInspector]public BoxCollider2D boxCollider;

        [HideInInspector] public TargetHeader header;

        private bool Initialized = false;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(Vector3.down * SpawnOffset + transform.position, 0.1f);
        }
        public void Init(GameObject obj,int camp=-1)
        {
            rb = obj.GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();

            bulletDetector = GetComponent<BulletDetector>();
            groundDetector = GetComponent<GroundDetector>();

            if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

            minimapIcon =Instantiate(Tool.PrefabManager.TargetMinimap.gameObject,transform).GetComponent<SpriteRenderer>();
            minimapIcon.transform.localPosition = Vector3.zero;
            if (camp>=0)
            {
                minimapIcon.color = Tool.SpriteManager.TargetToColor(camp);
            }
            if (obj.TryGetComponent(out Icontroller))
            {
                Icontroller.OnPostSync += OnSync;
            }
            else
            {
                Debug.LogError(gameObject.name + "未挂载同步组件");
            }
            InitHeader();
            Initialized = true;
        }
        private float headerOffset;
        private void InitHeader()
        {
            if (header) return;
            header = Instantiate(Tool.PrefabManager.TargetHeader.gameObject).GetComponent<TargetHeader>();
            header.transform.SetParent(Tool.Instance.WorldCanvas.transform);
            headerOffset = SpawnOffset + 1;
        }
        public void SetName(string text, Color color = default)
        {
            if (!header) InitHeader();
            if (text == NullName)
            {
                header.SetNameActive(false);
                return;
            }
            else
            {
                header.SetNameActive(true);
                header.SetNameText(text);
                header.SetNameColor(color);
            }
        }
        public void SetBarActive(bool active)
        {
            if (!header) InitHeader();
            header.gameObject.SetActive(active);
        }
        public void SetBarValue(float value)
        {
            if (!header) InitHeader();
            header.SetBarValue(value);
        }
        public void ShowEffects(List<EffectType> effects)
        {
            if (!header) InitHeader();
            header.ShowEffects(effects);
        }
        private void OnSync()
        {
            if (!Initialized) return;

            if (Icontroller.Info.faceRight) transform.localScale = R;
            else transform.localScale = L;

            if (!SetState) return;
            if (Icontroller.Info.isGrounded)
            {
                if (rb.velocity.x < 0.001f && rb.velocity.x > -0.001f)
                {
                    anim.SetInteger("State", 0);
                }
                else
                {
                    anim.SetInteger("State", 1);
                }
            }
            else
            {
                if (rb.velocity.y > 0.01f)
                {
                    anim.SetInteger("State", 2);
                }
                else
                {
                    anim.SetInteger("State", 3);
                }
            }
        }
        public void SetAsInvisiable()
        {
            if (TryGetComponent(out SpriteRenderer sr)) Destroy(sr);
            for(int i=0;i<transform.childCount;i++)
            {
                Destroy(transform.GetChild(i));
            }
            if (!header) InitHeader();
            header.gameObject.SetActive(false);
        }
        private void Update()
        {
            if (header) header.transform.position = transform.position + Vector3.up * headerOffset;
        }
        private void OnDestroy()
        {
            if (header!=null&&header.gameObject!=null) Destroy(header.gameObject);
        }
    }
}