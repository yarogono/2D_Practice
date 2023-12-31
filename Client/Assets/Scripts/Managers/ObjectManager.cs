using Google.Protobuf.Protocol;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : CustomSingleton<ObjectManager>
{
    public PlayerController MyPlayer { get; set; }
    readonly Dictionary<int, GameObject> _objects = new();

    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    public void Add(ObjectInfo info, bool isMyPlayer = false)
    {
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);

        if (objectType == GameObjectType.Player)
        {
            if (isMyPlayer)
            {
                AddMyPlayer(info);
            }
            else
            {
                AddEnemyPlayer(info);
            }
        }
        else if (objectType == GameObjectType.Item)
        {
            GameObject itemGameObject = ResourceManager.Instance.Instantiate($"Item/Weapon/{info.Name}");
            Transform itemTransform = itemGameObject.GetComponent<Transform>();

            Vec3 posInfo = info.PosInfo;
            itemTransform.position = new Vector3(posInfo.X, posInfo.Y, posInfo.Z);
        }
    }

    private void AddMyPlayer(ObjectInfo info)
    {
        GameObject cameraGameObject = ResourceManager.Instance.Instantiate("MainCamera");
        GameObject myPlayerGameObject = ResourceManager.Instance.Instantiate("MyPlayer");

        PlayerCameraController _controller = cameraGameObject.GetComponent<PlayerCameraController>();
        Transform target = myPlayerGameObject.GetComponent<Transform>();
        _controller.TargetSetting(target);

        myPlayerGameObject.name = info.Name;
        _objects.Add(info.ObjectId, myPlayerGameObject);

        PlayerController myPlayer = myPlayerGameObject.GetComponent<PlayerController>();
        myPlayer.Id = info.ObjectId;
        myPlayer.Name = info.Name;
        myPlayer.SetIsMine(true);
        PlayerSyncTransform playerSyncTransform = myPlayerGameObject.GetComponent<PlayerSyncTransform>();
        playerSyncTransform.PosInfo = info.PosInfo;
        MyPlayer = myPlayer;
    }

    private void AddEnemyPlayer(ObjectInfo info)
    {
        GameObject gameObject = ResourceManager.Instance.Instantiate("EnemyPlayer");

        _objects.Add(info.ObjectId, gameObject);

        PlayerController enemyPlayer = gameObject.GetComponent<PlayerController>();
        enemyPlayer.Id = info.ObjectId;
        enemyPlayer.Name = info.Name;
        enemyPlayer.SetIsMine(false);
        PlayerSyncTransform enemyPlayerSyncTransform = enemyPlayer.GetComponent<PlayerSyncTransform>();
        enemyPlayerSyncTransform.PosInfo = info.PosInfo;
    }

    public void Remove(int id)
    {
        GameObject gameObject = FindById(id);
        if (gameObject == null)
            return;

        _objects.Remove(id);
        Destroy(gameObject);
    }

    public GameObject FindById(int id)
    {
        _objects.TryGetValue(id, out GameObject gameObject);
        return gameObject;
    }

    public void Clear()
    {
        foreach (GameObject gameObject in _objects.Values)
        {
            Destroy(gameObject);
        }

        _objects.Clear();
        MyPlayer = null;
    }
}
