using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class EnemyTable : ScriptableObject
{
	public List<EnemyTableEntity> Table; // Replace 'EntityType' to an actual type that is serializable.
}
