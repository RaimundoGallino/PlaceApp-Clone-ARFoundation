#ifndef ANIMATED_INCLUDED
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
//#pragma exclude_renderers d3d11 gles
#define ANIMATED_INCLUDED


#if _ANIMATED_CENTRAL || _ANIMATED_DIAGONAL_CORNER || _ANIMATED_DIAGONAL_CENTRE
	uniform fixed _spread = 1;
	uniform fixed _offset = 0;
	uniform float3 _Centre;
	uniform float3 _BoxDirX;
	uniform float3 _BoxDirY;
	uniform float3 _BoxDirZ;
	uniform float3 _BoxExtent;
	static const float _offset1 = _offset*(1 + _spread);

#if _ANIMATED_DIAGONAL_CORNER || _ANIMATED_DIAGONAL_CENTRE
	static const float _diagExtent = length(_BoxExtent);
	uniform float3 _DiagPlane;
#endif
	inline float4 animTransition(float3 posWorld)
	{
		float transparency = 0;

#if _ANIMATED_CENTRAL
		float distX = -dot((posWorld - _Centre), _BoxDirX);
		float transparencyX = (_offset1* _BoxExtent.x - abs(distX) )/_spread;
		transparency = max(transparency, transparencyX);

		float distY = -dot((posWorld - _Centre), _BoxDirY);
		float transparencyY = (_offset1* _BoxExtent.y - abs(distY) ) /_spread;
		transparency = max(transparency, transparencyY);

		float distZ = -dot((posWorld - _Centre), _BoxDirZ);
		float transparencyZ = (_offset1* _BoxExtent.z - abs(distZ) ) /_spread;
		transparency = max(transparency, transparencyZ);
#endif
#if _ANIMATED_DIAGONAL_CORNER
		float dist = -dot((posWorld - _Centre), _DiagPlane);//*(1-2*_inverse);
		transparency = (dist + _offset1 * 2 *_diagExtent) /_spread;
#endif
#if _ANIMATED_DIAGONAL_CENTRE
		float dist = -dot((posWorld - _Centre), _DiagPlane);//*(1-2*_inverse);
		transparency = (- abs(dist) + _offset1 * _diagExtent) /_spread;
#endif
		float4 rgbcol = float4(0,0,0,0);

		rgbcol.a = clamp(transparency,0,1);

		return rgbcol;
	}

#define ANIM_FADE(posWorld) animTransition(posWorld);
#endif

#endif // ANIMATED_INCLUDED