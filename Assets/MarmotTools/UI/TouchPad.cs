﻿using UnityEngine;

namespace MyMarmot.Tools
{
    public class TouchPad : MonoBehaviour
    {
        private RectTransform _touchPad;
        private int _touchId = -1;//컨트롤로영역 입력구분 아이디
        private Vector3 _starPos = Vector3.zero;//입력시작좌표
        public float _dragRadius = 60f;//컨트롤러반지름

        private bool _buttonPressed = false;

        [SerializeField]
        private EventVector3Float _OnTouchDrag;

        public EventVector3Float m_OnTouchDrag { get => _OnTouchDrag; }

        public float m_TimeTick { get;private set; }

        private void Start()
        {
            _touchPad = GetComponent<RectTransform>();
            _starPos = _touchPad.position;
        }
        public void ButtonDown()
        {
            _buttonPressed = true;
        }
        public void ButtonUp()
        {
            _buttonPressed = false;
            HandleInput(_starPos);
        }

        private void Update()
        {
            m_TimeTick = Time.deltaTime;
            HandleTouchInput();
            HandleInput(Input.mousePosition);
        }

        private void HandleTouchInput()
        {
            //터치 아이디를 매기는 번호
            int i = 0;
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)//터치입력을 하나씩 조회
                {
                    i++;//터치아이디 번호 증가
                    Vector3 touchPos = new Vector3(touch.position.x, touch.position.y);//현재 터치좌표
                    if (touch.phase == TouchPhase.Began)//터치입력시작시
                    {
                        if (touch.position.x <= (_starPos.x + _dragRadius))//그리고 터치의좌표가 현재방향키 범위내에 있다면
                        {
                            _touchId = i;//이 터치 아이디를 기준으로 방향컨트롤러를 조작하도록 합니다.
                        }

                    }

                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)//터치입력이 움직였다거나 가만히있는 상황이라면
                    {
                        if (_touchId == i)//터치 아이디 지정된 경우에만
                        {
                            HandleInput(touchPos);//좌표 입력을 받아들입니다
                        }
                    }
                    if (touch.phase == TouchPhase.Ended)//터치입력끝났는데
                    {
                        if (_touchId == i)//입력받고자했던 터치아이디라면
                        {
                            _touchId = -1;//터치 아이디를 해제합니다
                        }
                    }
                }
            }
        }

        void HandleInput(Vector3 input)
        {
            if (_buttonPressed)//버튼이눌리면
            {
                Vector3 diffVector = (input - _starPos);//ㅂ방향컨트롤러 기준좌표로부터 입력받은 좌표가 얼마나 먼지 구함
                if (diffVector.sqrMagnitude > _dragRadius * _dragRadius)//입력 지점과 기준좌표의 거리를 비교합니다.만약 최대치보다 크다면
                {
                    diffVector.Normalize();//방향벡터거리를 1로만든다

                    _touchPad.position = _starPos + diffVector * _dragRadius;//그리고 방향컨트롤러는 최대치만큼만 움직이게합니다
                }
                else//입력지점과 기준좌표가 최대치보다 안크면
                {
                    _touchPad.position = input;
                }
            }
            else//버튼에서 손을 때면
            {
                _touchPad.position = _starPos;//방향키를 원위치로 돌려놓음
            }
            Vector3 diff = _touchPad.position - _starPos;//방향키와 기준점의 차이구함

            Vector2 normDiff = new Vector3(diff.x / _dragRadius, diff.y / _dragRadius);//방향키를 유지한채로 거리를 나눠 방향만을 얻어냄

            m_OnTouchDrag?.Invoke(new Vector3(normDiff.x, normDiff.y, 0), m_TimeTick);

        }


    }

}