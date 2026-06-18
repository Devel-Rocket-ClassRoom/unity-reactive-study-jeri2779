using UnityEngine;

namespace DIStudy.LifetimeLab
{
    /// <summary>
    /// 인스턴스 생성 순서를 눈으로 확인하기 위한 일련번호 발급기.
    /// </summary>
    public static class InstanceIdFactory
    {
        private static int s_Next;

        public static string Next(string prefix)
        {
            return $"{prefix}-{++s_Next}";
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlay()
        {
            s_Next = 0;
        }
    }
}
