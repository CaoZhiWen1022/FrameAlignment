using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FGUIFrame
{
    /// <summary>
    /// 定时器管理
    /// </summary>
    public class TimerMgr : MonoBehaviour
    {
        private class FuncObj
        {
            public Action rawFun;
            public Coroutine coroutine;
            public object target;
        }

        private List<FuncObj> funcList = new List<FuncObj>();

        /// <summary>
        /// 只执行一次 不支持匿名函数
        /// </summary>
        /// <param name="delay">延迟时间：单位秒 传0下一帧执行</param>
        /// <param name="callback">回调方法</param>
        /// <param name="target">回调方法所属对象</param>
        public void Once(float delay, Action callback, object target)
        {
            var funObj = new FuncObj
            {
                rawFun = callback,
                target = target
            };
            funObj.coroutine = StartCoroutine(OnceCoroutine(delay, callback, target, funObj));
            this.funcList.Add(funObj);
        }

        private IEnumerator OnceCoroutine(float delay, Action callback, object target, FuncObj funObj)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
            callback?.Invoke();
            this.funcList.Remove(funObj);
        }

        /// <summary>
        /// 定时循环执行 不支持匿名函数
        /// </summary>
        /// <param name="delay">间隔时间</param>
        /// <param name="callback">回调函数</param>
        /// <param name="target">回调方法所属对象</param>
        public void Loop(float delay, Action callback, object target)
        {
            var funObj = new FuncObj
            {
                rawFun = callback,
                target = target
            };
            funObj.coroutine = StartCoroutine(LoopCoroutine(delay, callback, target, funObj));
            this.funcList.Add(funObj);
        }

        private IEnumerator LoopCoroutine(float delay, Action callback, object target, FuncObj funObj)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                callback?.Invoke();
            }
        }

        /// <summary>
        /// 清除定时器
        /// </summary>
        public void Clear(Action callback, object target)
        {
            var funObj = this.funcList.FirstOrDefault(v => v.rawFun == callback && v.target == target);
            if (funObj != null)
            {
                if (funObj.coroutine != null)
                {
                    StopCoroutine(funObj.coroutine);
                }
                this.funcList.Remove(funObj);
            }
        }

        /// <summary>
        /// 清除所有定时器
        /// </summary>
        public void ClearAll(object target)
        {
            for (int i = this.funcList.Count - 1; i >= 0; i--)
            {
                if (this.funcList[i].target == target)
                {
                    this.Clear(this.funcList[i].rawFun, this.funcList[i].target);
                }
            }
        }
    }
}

