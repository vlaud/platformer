using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ComponentTypeFinder
{
    /// <summary>
    /// 인터페이스의 타입과 인터페이스를 구현하는 MonoBehaviour 딕셔너리
    /// </summary>
    private static Dictionary<Type, List<MonoBehaviour>> _components = new Dictionary<Type, List<MonoBehaviour>>();

    /// <summary>
    /// 신 완전 로드 후, 게임 오브젝트의 인터페이스들을 찾아 딕셔너리에 등록합니다.
    /// </summary>
    public static void Initialize()
    {
        _components.Clear();
        // 모든 MonoBehaviour 오브젝트 검색
        MonoBehaviour[] allBehaviours = GameObject.FindObjectsByType<MonoBehaviour>();

        foreach (var behaviour in allBehaviours)
        {
            // 이 MonoBehaviour가 구현하는 모든 인터페이스를 가져온다.
            var interfaces = behaviour.GetType().GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                // 딕셔너리에 인터페이스 타입이 없으면 새로 추가
                if (!_components.ContainsKey(interfaceType))
                {
                    _components[interfaceType] = new List<MonoBehaviour>();
                }
                // 딕셔너리에 MonoBehaviour 인스턴스 추가
                _components[interfaceType].Add(behaviour);
            }
        }
    }

    /// <summary>
    /// 트랜스폼에서 T를 상속받은 타입을 찾습니다.
    /// </summary>
    /// <param name="transform">트랜스폼</param>
    /// <returns></returns>
    public static Type GetType<T>(this Transform transform) where T : class
    {
        // T 타입을 상속받은 MonoBehaviour를 transform 내에서 찾습니다.
        var components = transform.GetComponents<MonoBehaviour>()
                                  .Where(c => c is T)
                                  .ToArray();

        if (components == null || components.Length == 0)
            return null;

        // 가장 구체적인 타입을 찾기 위해 OrderByDescending로 깊이 정렬
        return components
            .Select(c => c.GetType())
            .OrderByDescending(type => GetInheritanceDepth<T>(type))
            .FirstOrDefault();
    }

    /// <summary>
    /// 트랜스폼에서 T를 상속받은 개체를 찾아 반환합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static T GetMostDerivedComponent<T>(this Transform transform) where T : class
    {
        var component = transform.GetComponents<MonoBehaviour>()
                                  .Where(c => c is T)
                                  .Cast<T>()
                                  .OrderByDescending(c => GetInheritanceDepth<T>(c.GetType()))
                                  .FirstOrDefault();

        return component;
    }

    /// <summary>
    /// 게임오브젝트에서 T를 상속받은 개체를 찾아 반환합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T GetMostDerivedComponent<T>(this GameObject gameObject) where T : class
    {
        var component = gameObject.GetComponents<MonoBehaviour>()
                                  .Where(c => c is T)
                                  .Cast<T>()
                                  .OrderByDescending(c => GetInheritanceDepth<T>(c.GetType()))
                                  .FirstOrDefault();

        return component;
    }

    /// <summary>
    /// 타입의 상속 깊이를 계산합니다. T를 상속받은 타입의 깊이를 계산합니다.
    /// </summary>
    /// <param name="type">상속받은 클래스 타입 </param>
    /// <returns></returns>
    private static int GetInheritanceDepth<T>(Type type) where T : class
    {
        // 깊이는 0부터 시작합니다.
        int depth = 0;

        // type이 null이 아니고, T 타입이 아니고, object 타입이 아닐 때까지 반복합니다.
        while (type != null && type != typeof(T) && type != typeof(object))
        {
            depth++;
            type = type.BaseType;
        }
        return depth;
    }

    /// <summary>
    /// 모든 MonoBehaviour 오브젝트를 검색하여 특정 인터페이스를 구현하는 오브젝트들을 찾습니다.
    /// </summary>
    /// <typeparam name="T">인터페이스</typeparam>
    /// <returns></returns>
    public static List<T> FindAllImplementing<T>() where T : class
    {
        Type interfaceType = typeof(T);

        // 딕셔너리에 해당 타입이 있는지 확인
        if (_components.TryGetValue(interfaceType, out var componentList))
        {
            // 딕셔너리에 있으면 리스트를 캐스팅해서 반환 
            return componentList.Cast<T>().ToList();
        }

        List<MonoBehaviour> results = new List<MonoBehaviour>();

        // 딕셔너리에 없으면 새로 검색
        MonoBehaviour[] allBehaviours = UnityEngine.Object.FindObjectsByType<MonoBehaviour>();

        foreach (var behaviour in allBehaviours)
        {
            if (behaviour is T)
            {
                results.Add(behaviour);
            }
        }
        // 검색 결과를 딕셔너리에 저장
        _components[interfaceType] = results;

        // 결과를 캐스팅해서 반환
        return results.Cast<T>().ToList();
    }

    /// <summary>
    /// 모든 MonoBehaviour 오브젝트를 검색하여 특정 인터페이스를 구현하는 오브젝트 한개를 찾습니다. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FindFirstImplementing<T>() where T : class
    {
        return FindAllImplementing<T>().FirstOrDefault();
    }
}