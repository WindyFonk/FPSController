using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolForShell : MonoBehaviour
{
    public static PoolForShell Instance;
    public Transform shellPoint;
    public GameObject shellPrefab;
    public int poolSize = 10;

    public List<GameObject> shellPool = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject shell = Instantiate(shellPrefab, Vector3.zero, Quaternion.identity);
            shell.SetActive(false);
            shellPool.Add(shell);
        }
    }

    public GameObject GetShell()
    {
        foreach (GameObject shell in shellPool)
        {
            if (!shell.activeInHierarchy)
            {
                shell.SetActive(true);
                return shell;
            }
        }
        return null;
    }

    public void ReturnShellToPool(GameObject shell)
    {
        shell.SetActive(false);
    }
}
