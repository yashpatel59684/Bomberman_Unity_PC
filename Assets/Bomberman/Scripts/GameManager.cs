using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameManager : GenericSingletonClass<GameManager>
{
    [SerializeField] GameObject tutorialPanel, resultPanel;
    [SerializeField] MapGenerator mapGenerator;
    [SerializeField] Transform playerPrefab, enemyPrefab, bombPrefab, bombBlastPrefab;
    [Range(3, 10)]
    [SerializeField] int maxEnemyAllow = 5;
    [Range(2, 7)]
    [SerializeField] int minEnemyCriticalCount = 2;
    internal int MaxEnemyAllow => maxEnemyAllow;
    internal int MinEnemyCriticalCount => minEnemyCriticalCount;
    [Range(1, 10)]
    [SerializeField] int tutorialShowRange = 5;
    [Range(1, 10)]
    [SerializeField] int bombBlastRange = 3;
    [Range(0, 1)]
    [SerializeField] float bombNextBlastWait = .01f;
    Transform bomb;
    internal Transform PlayerObj { get; private set; }
    internal List<Transform> EnemyObj { get; private set; }
    bool isGameDone = false;
    int totalWinGames = 0;
    GameManager() { }
    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        if (IsTutorialShown)
        {
            Destroy(tutorialPanel);
        }
        else
        {
            IsTutorialShown = true;
            tutorialPanel.SetActive(true);
            Destroy(tutorialPanel, tutorialShowRange);
        }
        RestartGame();
    }
    internal void RestartGame()
    {
        isGameDone = false;
        totalWinGames = TotalWinGames;
        EnemyObj = new List<Transform>();
        StopAllCoroutines();
        if (bomb != null)
        {
            Destroy(bomb.gameObject);
        }
        mapGenerator.GenerateMap();
    }
    internal void GeneratePlayer(Transform transform, Vector2 vector2)
    {
        if (!IsPlayerAlive)
        {
            PlayerObj = Instantiate(playerPrefab, transform);
            PlayerObj.transform.position = vector2;

        }
    }
    internal void PlayerDie()
    {
        Debug.Log($"Player Die");
        StartCoroutine(CheckIsPlayerWin());
    }
    void DestroyPlayer()
    {
        if (IsPlayerAlive)
        {
            Destroy(PlayerObj.gameObject);
        }
    }
    bool IsPlayerAlive => PlayerObj != null;
    internal bool IsGameWin => TotalWinGames > totalWinGames;
    internal void GenerateEnemy(Transform transform, Vector2 vector2)
    {
        Transform temp = Instantiate(enemyPrefab, transform);
        temp.position = vector2;
        if (EnemyObj == null)
        {
            EnemyObj = new List<Transform>();
        }
        EnemyObj.Add(temp);
    }
    internal void RemoveEnemy(Transform transform)
    {
        if (EnemyObj.Contains(transform)) EnemyObj.Remove(transform);
        CheckAllEnemyDie();
    }

    void CheckAllEnemyDie()
    {
        if (EnemyObj.Count <= 0)
        {
            Debug.Log($"All Enemy Die");
            StartCoroutine(CheckIsPlayerWin());
        }
    }
    IEnumerator CheckIsPlayerWin()
    {
        if (isGameDone) yield break;
        isGameDone = true;
        yield return new WaitForSeconds((bombNextBlastWait * bombBlastRange) + 2);
        if (IsPlayerAlive)
        {
            DestroyPlayer();
            ++TotalWinGames;
            Debug.Log($"Player win");
        }
        else
        {
            Debug.Log($"Player Lose");
        }
        ++TotalPlayedGames;
        //mapGenerator.DeleteOldMap();
        resultPanel.SetActive(true);
        StopAllCoroutines();
    }
    internal void PlaceBomb(Vector3 position)
    {
        if (bomb == null)
        {
            Vector3 pos = GetNearestAbsoluteLocation(position);
            bomb = Instantiate(bombPrefab, pos, Quaternion.identity);
        }
    }
    internal void BlasstBomb()
    {
        if (bomb == null) return;
        if (IsPlayerAlive && GetNearestAbsoluteLocation(PlayerObj.position) == bomb.position)
        {
            PlayerDie();
        }
        StartCoroutine(BlasstBombCheck(Vector3.up));
        StartCoroutine(BlasstBombCheck(Vector3.down));
        StartCoroutine(BlasstBombCheck(Vector3.right));
        StartCoroutine(BlasstBombCheck(Vector3.left));
        Destroy(bomb.gameObject);
    }
    IEnumerator BlasstBombCheck(Vector3 direction)
    {
        Vector3 fromPosition = bomb.transform.position;
        int rangeCounter = 0;
        yield return null;
        while (rangeCounter < bombBlastRange)
        {
            yield return new WaitForSeconds(bombNextBlastWait);
            if (IsWayClearToBlast(fromPosition, direction, out Collider2D collider2D))
            {
                //Debug.Log($"{fromPosition} {direction} {collider2D?.gameObject?.name}");
                fromPosition += direction;
                ++rangeCounter;
                Instantiate(bombBlastPrefab, fromPosition, Quaternion.identity);
                if ((collider2D?.CompareTag("Brick") ?? false) || (collider2D?.CompareTag("Enemy") ?? false) || (collider2D?.CompareTag("Player") ?? false))
                {
                    if (collider2D.CompareTag("Enemy"))
                    {
                        RemoveEnemy(collider2D.transform);
                    }
                    yield break;
                }
            }
            else
            {
                yield break;
            }
        }
    }
    bool IsWayClearToBlast(Vector3 position, Vector3 towords, out Collider2D collider2D)
    {
        RaycastHit2D[] raycastHit2D = Physics2D.RaycastAll(position, towords, 1f);
        collider2D = null;
        if (raycastHit2D.Length > 0)
        {
            foreach (var item in raycastHit2D)
            {
                //Debug.Log($"{position} {towords} {item.collider.gameObject.name}");
                collider2D = item.collider;
                if (item.collider.CompareTag("Wall"))
                {
                    return false;
                }
            }
        }
        return true;
    }
    internal Vector3 GetNearestAbsoluteLocation(Vector3 position)
    {
        return new Vector3(MathF.Round(position.x), MathF.Round(position.y), 0);
    }
    #region Playerprefs
    internal int TotalPlayedGames
    {
        get => PlayerPrefs.GetInt(nameof(TotalPlayedGames), 0);
        private set
        {
            PlayerPrefs.SetInt(nameof(TotalPlayedGames), value);
            PlayerPrefs.Save();
        }
    }
    internal int TotalWinGames
    {
        get => PlayerPrefs.GetInt(nameof(TotalWinGames), 0);
        private set
        {
            PlayerPrefs.SetInt(nameof(TotalWinGames), value);
            PlayerPrefs.Save();
        }
    }
    internal bool IsTutorialShown
    {
        get => PlayerPrefs.GetInt(nameof(IsTutorialShown), 0) == 1;
        private set
        {
            PlayerPrefs.SetInt(nameof(IsTutorialShown), value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    #endregion
}
