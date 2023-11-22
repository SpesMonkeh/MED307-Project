// Copyright © Christian Holm Christensen
// 28/09/2023

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace P307.Runtime.Core
{
	[DisallowMultipleComponent][AddComponentMenu("307/Core/Core Component")]
	public sealed class CoreComponent : MonoBehaviour
	{
		public static event Action AllCoreComponentsInitialized = delegate { };

		readonly Dictionary<IP307CoreComponent, bool> initializedComponents = new();

		void Awake()
		{
			DontDestroyOnLoad(gameObject);

			if (AddRequiredCoreComponents() is false) return;

			var allInitialized = initializedComponents.All(pair => pair.Value);

			if (allInitialized is false) return;

			AllCoreComponentsInitialized?.Invoke();
			Debug.Log("Core Component Awake()");
		}

		bool AddRequiredCoreComponents()
		{
			initializedComponents.Clear();

			HashSet<Type> coreTypes = GetClassesImplementingInterface(typeof(IP307CoreComponent))
				.ToHashSet();
			
			HashSet<Type> currentComponents = gameObject.GetComponents<IP307CoreComponent>()
				.Select(t => 
					t.GetType())
				.ToHashSet();
			
			HashSet<Type> similarTypes = coreTypes.Intersect(currentComponents)
				.ToHashSet();

			if (similarTypes.Count is 0)
				AddCoreComponents(coreTypes);
			else
				AddMissingCoreComponents(coreTypes, similarTypes);

			currentComponents = gameObject.GetComponents<IP307CoreComponent>()
				.Select(t => 
					t.GetType())
				.ToHashSet();
			
			similarTypes = coreTypes.Intersect(currentComponents)
				.ToHashSet();
			
			Debug.Log("Core Component AddRequiredComponents()");

			return similarTypes.Count == coreTypes.Count;
		}

		void AddCoreComponents(HashSet<Type> coreTypes)
		{
			foreach (Type addedComponent in coreTypes)
			{
				Component component = gameObject.AddComponent(addedComponent);
				InitializeCoreComponent(component);
			}
		}

		void AddMissingCoreComponents(IEnumerable<Type> coreTypes, HashSet<Type> similarTypes)
		{
			foreach (var addedComponent in coreTypes
				         .SelectMany(coreType =>
					         from similarType in similarTypes
					         where similarType != coreType
					         select gameObject.AddComponent(coreType)
				         ))
				InitializeCoreComponent(addedComponent);
		}

		void InitializeCoreComponent(Component component)
		{
			if (component is not IP307CoreComponent coreComponent)
				return;
			if (initializedComponents.TryAdd(coreComponent, false) is false)
				return;
			initializedComponents[coreComponent] = coreComponent.Initialize();
		}

		static IEnumerable<Type> GetClassesImplementingInterface(Type interfaceType)
		{
			if (interfaceType is null)
				throw new NullReferenceException($"The provided type was null.");
			if (interfaceType.IsInterface is false)
				throw new ArgumentException($"The type provided, {nameof(interfaceType)}, must be an interface.");

			List<Type> implementingClasses = Enumerable.Empty<Type>().ToList();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var current in assemblies)
			{
				Type[] types = current.GetTypes();
				implementingClasses
					.AddRange(types
						.Where(t =>
							interfaceType.IsAssignableFrom(t)
							&& t.IsInterface is false)
					);
			}

			return implementingClasses;
		}
	}
}