using System;
using System.Collections.Generic;
using UnityEngine;

namespace View.UI.Popups.ContentPopup
{
    public class UIContentPopup : UIPopupViewBase
    {
        [SerializeField] private RectTransform _viewportTransform;
        [SerializeField] private RectTransform _contentTransform;

        private readonly LinkedList<ItemData> _hiddenItemsHead = new();
        private readonly LinkedList<ItemData> _displayedItems = new();
        private readonly LinkedList<ItemData> _hiddenItemsTail = new();

        private int _columnsCount = 3;
        private Vector2 _contentSize;
        private Vector2 _viewPortSize;
        private Vector2 _contentTransformPosition;

        public RectTransform ContentTransform => _contentTransform;

        protected override void Awake()
        {
            base.Awake();
            
            _contentTransformPosition = _contentTransform.anchoredPosition;
        }

        private void Update()
        {
            var prevContentTransformPosition = _contentTransformPosition;
            _contentTransformPosition = _contentTransform.anchoredPosition;

            if (_contentTransformPosition.y < prevContentTransformPosition.y)
            {
                ProcessScrollForward();
            }
            else if (_contentTransformPosition.y > prevContentTransformPosition.y)
            {
                ProcessScrollBackward();
            }
        }

        public void Setup(int columnsCount, int popupWidth, int popupHeight)
        {
            _viewPortSize = _viewportTransform.rect.size;

            _columnsCount = columnsCount;
            SetPopupSize(popupWidth, popupHeight);
        }

        public void AddItem(IUIContentPopupItem item)
        {
            item.RectTransform.SetParent(_contentTransform);
            SetItemActive(item, true);

            var allItemsCount = _hiddenItemsHead.Count + _displayedItems.Count + _hiddenItemsTail.Count;
            SetItemPosition(item, allItemsCount);

            var itemData = new ItemData(item);

            if (-itemData.EndCoord > _contentSize.y)
            {
                SetContentHeight(-itemData.EndCoord);
            }

            _hiddenItemsTail.AddLast(new ItemData(item));

            TryShowTailHiddenItem();
        }

        public void ClearContent()
        {
            foreach (Transform child in _contentTransform)
            {
                Destroy(child.gameObject);
            }

            _hiddenItemsHead.Clear();
            _displayedItems.Clear();
            _hiddenItemsTail.Clear();

            SetContentHeight(0);
        }

        public void ResetContentPosition()
        {
            _contentTransform.anchoredPosition = Vector2.zero;
        }

        public void SetContentYPosition(float position)
        {
            var pos = _contentTransform.anchoredPosition;
            pos.y = position;
            _contentTransform.anchoredPosition = pos;
        }

        private void ProcessScrollForward()
        {
            ProcessItemsAction(TryShowHeadHiddenItem);
            ProcessItemsAction(TryHideTailItem);
        }

        private void ProcessScrollBackward()
        {
            ProcessItemsAction(TryShowTailHiddenItem);
            ProcessItemsAction(TryHideHeadItem);
        }

        private static void ProcessItemsAction(Func<bool> action)
        {
            var safeCounter = 100;

            while (action() && safeCounter > 0)
            {
                safeCounter--;
            }

            if (safeCounter <= 0)
            {
                Debug.LogError($"{nameof(safeCounter)} alert!");
            }
        }

        private bool TryHideTailItem()
        {
            if (_displayedItems.Count > 0)
            {
                var lastItemData = _displayedItems.Last.Value;

                if (ShouldHideItemAtTail(lastItemData))
                {
                    SetItemActive(lastItemData.Item, false);

                    _displayedItems.RemoveLast();
                    _hiddenItemsTail.AddFirst(lastItemData);

                    return true;
                }
            }

            return false;
        }

        private bool TryShowTailHiddenItem()
        {
            if (_hiddenItemsTail.Count > 0)
            {
                var firstHiddenItemData = _hiddenItemsTail.First.Value;

                if (ShouldHideItemAtTail(firstHiddenItemData) == false)
                {
                    SetItemActive(firstHiddenItemData.Item, true);

                    _hiddenItemsTail.RemoveFirst();
                    _displayedItems.AddLast(firstHiddenItemData);

                    return true;
                }
            }

            return false;
        }

        private bool TryHideHeadItem()
        {
            if (_displayedItems.Count > 0)
            {
                var firstItemData = _displayedItems.First.Value;

                if (ShouldHideItemAtHead(firstItemData))
                {
                    SetItemActive(firstItemData.Item, false);

                    _displayedItems.RemoveFirst();
                    _hiddenItemsHead.AddLast(firstItemData);

                    return true;
                }
            }

            return false;
        }

        private bool TryShowHeadHiddenItem()
        {
            if (_hiddenItemsHead.Count > 0)
            {
                var lastHiddenItemData = _hiddenItemsHead.Last.Value;

                if (ShouldHideItemAtHead(lastHiddenItemData) == false)
                {
                    SetItemActive(lastHiddenItemData.Item, true);

                    _hiddenItemsHead.RemoveLast();
                    _displayedItems.AddFirst(lastHiddenItemData);

                    return true;
                }
            }

            return false;
        }

        private bool ShouldHideItemAtTail(ItemData itemData)
        {
            return -(itemData.StartCoord + _contentTransformPosition.y) > _viewPortSize.y;
        }

        private bool ShouldHideItemAtHead(ItemData itemData)
        {
            return -(itemData.EndCoord + _contentTransformPosition.y) < 0;
        }

        private Vector2 SetItemPosition(IUIContentPopupItem item, int itemIndex)
        {
            var position = new Vector2Int(itemIndex % _columnsCount, -itemIndex / _columnsCount) * item.Size;

            item.RectTransform.anchoredPosition = position;

            return position;
        }

        private void SetContentHeight(float height)
        {
            var tempSize = PopupTransform.sizeDelta;
            _contentTransform.sizeDelta = new Vector2(tempSize.x, height);
            _contentSize = _contentTransform.sizeDelta;
        }

        private static void SetItemActive(IUIContentPopupItem item, bool isActive)
        {
            item.RectTransform.gameObject.SetActive(isActive);
        }

        private struct ItemData
        {
            public readonly IUIContentPopupItem Item;
            public readonly float StartCoord;
            public readonly float EndCoord;

            public ItemData(IUIContentPopupItem item)
            {
                Item = item;

                var anchoredPosition = Item.RectTransform.anchoredPosition;
                StartCoord = anchoredPosition.y;
                EndCoord = anchoredPosition.y - Item.Size.y;
            }
        }
    }
}