using UnityEngine;
using System.Collections.Generic;



[ExecuteInEditMode]
[AddComponentMenu( "NGUI/UI/Filled Texture" )]
public class UIFilledTexture : UITexture
{

    Rect lastPosition;

    public Texture tex;

    [SerializeField]
    public Rect Position;
    [SerializeField]
    protected Rect mOuterUV;

    [SerializeField]
    string sprite_name = "";
	
	Material transMaterial;

    protected override void Awake()
    {
        material = null;
    }


    void Start()
    {
        updateSprite();
    }


    public string spriteName
    {
        get { return sprite_name; }
        set
        {
            if(  value == sprite_name )
            {
                return;
            }

            if( string.IsNullOrEmpty( value ) )
            {

            }

            sprite_name = value;
            updateSprite();
        }
    }



    public void updateSprite()
    {
        if (tex == null)
        {
            tex = ResManager.Singleton.GetIconByName(spriteName);
        }
        //Debug.LogError( string.Format( "SpriteName = {0} Texture = {1}" , spriteName , null != tex ) );
        if( null != tex )
        {
            if( null == transMaterial )
            {
                transMaterial = new Material( Shader.Find( "Unlit/Transparent Colored" ) );
            }
			material = transMaterial;
            material.SetTexture( "_MainTex" , tex );
            Position.width = tex.width;
            Position.height = tex.height;
            return;
        }
        material = null;
    }


    public override bool OnUpdate()
    {
        if( lastPosition != Position )
        {
            lastPosition = Position;
            UpdateUVs( false );
            return true;
        }
        UpdateUVs( false );
        return false;
    }


    void UpdateUVs( bool force )
    {
        if( null == mainTexture )
        {
            return;
        }

        Texture tex = mainTexture;
        mOuterUV = NGUIMath.ConvertToTexCoords( Position , tex.width , tex.height );
    }



    public override void MakePixelPerfect()
    {
        Vector3 scale = transform.localScale;
        scale.x = Position.width;
        scale.y = Position.height;
        scale.z = 1;
        transform.localScale = scale;
    }


    override public void OnFill( BetterList<Vector3> verts , BetterList<Vector2> uvs , BetterList<Color32> cols )
    {
        Vector2 uv0 = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
        Vector2 uv1 = new Vector2(mOuterUV.xMax, mOuterUV.yMax);

        verts.Add( new Vector3( 1f , 0f , 0f ) );
        verts.Add( new Vector3( 1f , -1f , 0f ) );
        verts.Add( new Vector3( 0f , -1f , 0f ) );
        verts.Add( new Vector3( 0f , 0f , 0f ) );

        uvs.Add( uv1 );
        uvs.Add( new Vector2( uv1.x , uv0.y ) );
        uvs.Add( uv0 );
        uvs.Add( new Vector2( uv0.x , uv1.y ) );

        cols.Add( color );
        cols.Add( color );
        cols.Add( color );
        cols.Add( color );
    }


}

