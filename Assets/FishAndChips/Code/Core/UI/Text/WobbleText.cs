using UnityEngine;
using TMPro;

namespace FishAndChips
{
    public class WobbleText : MonoBehaviour
    {
		#region -- Inspector --
		public float SinValue = 1.1f;
		public float CosValue = 0.8f;
		#endregion

		#region -- Private Member Vars --
		private TMP_Text _textMesh;
		private Mesh _mesh;
		private Vector3[] _vertices;
		#endregion

		#region -- Private Methods --
		private Vector2 Wobble(float time)
		{
			return new Vector2(Mathf.Sin(time * SinValue), Mathf.Cos(time * CosValue));
		}

		private void Start()
		{
			_textMesh = GetComponent<TMP_Text>();
		}

		private void Update()
		{
			_textMesh.ForceMeshUpdate();
			_mesh = _textMesh.mesh;
			_vertices = _mesh.vertices;

			for (int i = 0; i < _vertices.Length; i++)
			{
				Vector3 offset = Wobble(Time.time + i);
				_vertices[i] = _vertices[i] + offset;
			}

			_mesh.vertices = _vertices;
			_textMesh.canvasRenderer.SetMesh(_mesh);
			
		}
		#endregion
	}
}
