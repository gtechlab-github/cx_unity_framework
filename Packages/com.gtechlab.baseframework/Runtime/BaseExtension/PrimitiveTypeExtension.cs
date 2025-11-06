using UnityEngine;

public static class PrimitiveTypeExtension {
    public static Color WithAlpha(this Color color, float alpha) {
        return new Color(color.r, color.g, color.b, alpha);
    }
    
    public static Color WithColor(this Color color, Color other) {
        return new Color(other.r, other.g, other.b, color.a);
    }

    public static Vector3 WithX(this Vector3 vector, float x) {
        return new Vector3(x, vector.y, vector.z);
    }

    public static Vector3 WithY(this Vector3 vector, float y) {
        return new Vector3(vector.x, y, vector.z);
    }

    public static Vector3 WithZ(this Vector3 vector, float z) {
        return new Vector3(vector.x, vector.y, z);
    }
}

[System.Serializable]
public struct TVector2 {
    public float x;
    public float y;

    public TVector2(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public TVector2(Vector2 v) {
        this.x = v.x;
        this.y = v.y;
    }

    public static implicit operator Vector2(TVector2 v) {
        return new Vector2(v.x, v.y);
    }

    public static implicit operator TVector2(Vector2 v) {
        return new TVector2(v.x, v.y);
    }
}

[System.Serializable]
public struct TVector3 {
    public float x;
    public float y;
    public float z;

    public TVector3(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public TVector3(Vector3 v) {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }

    public static implicit operator Vector3(TVector3 v) {
        return new Vector3(v.x, v.y, v.z);
    }

    public static implicit operator TVector3(Vector3 v) {
        return new TVector3(v.x, v.y, v.z);
    }
    public static bool Equals (TVector3 a, TVector3 b) {
        float epsilon = Mathf.Epsilon;
        return Mathf.Abs (a.x - b.x) < epsilon && Mathf.Abs (a.y - b.y) < epsilon && Mathf.Abs (a.z - b.z) < epsilon;
    }

    public bool Equals(TVector3 other) {
        return Equals(this, other);
    }
}

[System.Serializable]
public struct TVector2Int {
    public int x;
    public int y;

    public TVector2Int(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public TVector2Int(Vector2Int v) {
        this.x = v.x;
        this.y = v.y;
    }

    public static implicit operator Vector2Int(TVector2Int v) {
        return new Vector2Int(v.x, v.y);
    }

    public static implicit operator TVector2Int(Vector2Int v) {
        return new TVector2Int(v.x, v.y);
    }
}

[System.Serializable]
public struct TVector3Int {
    public int x;
    public int y;
    public int z;

    public TVector3Int(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public TVector3Int(Vector3Int v) {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }

    public static implicit operator Vector3Int(TVector3Int v) {
        return new Vector3Int(v.x, v.y, v.z);
    }

    public static implicit operator TVector3Int(Vector3Int v) {
        return new TVector3Int(v.x, v.y, v.z);
    }
}