float4x4 World;
float4x4 View;
float4x4 Projection;

sampler textureSampler;

struct PixelInput {
    float2 uv : TEXCOORD0;
};

float4 outline(PixelInput input) : COLOR0 {
	float4 col;
	
	float2 cen = input.uv;
	float4 mid = tex2D(textureSampler,cen+float2(0,0));

	if(input.uv.y < 1) {
		float dis = 0.005;
		
		float4 up = tex2D(textureSampler,cen+float2(-dis,0));
		float4 dn = tex2D(textureSampler,cen+float2(dis,0));
		float4 le = tex2D(textureSampler,cen+float2(0,-dis));
		float4 ri = tex2D(textureSampler,cen+float2(0,dis));

		col = tex2D(textureSampler,cen+float2(0,0));
		
		/*
		if (col.r != up.r || col.g != up.g || col.b != up.b || col.r != le.r || col.g != le.g || col.b != le.b) {
			col = 0;
		}
		*/
		
		//float4 edge = 4*abs(up-dn)-abs(le-ri); // edge
		//if ((edge.r + edge.g + edge.b)/3 > 0.4)
		//	col = col*0.5; 
		
		/*
		if ((col.r + col.g + col.b)/3 < 0.2) {
			col = col*0.8;
		}
		if ((col.r + col.g + col.b)/3 < 0.4) {
			col = col*0.8;
		}
		if ((col.r + col.g + col.b)/3 < 0.6) {
			col = col*0.8;
		}
		if ((col.r + col.g + col.b)/3 < 0.8) {
			col = col*0.8;
		}
		*/
			
		/*
		if (col.r >= col.g && col.r >= col.b) {
			col.r = 1;
			col.g = 0;
			col.b = 0;
		}
		if (col.g >= col.r && col.g >= col.b) {
			col.r = 0;
			col.g = 1;
			col.b = 0;
		}
		if (col.b >= col.g && col.b >= col.r) {
			col.r = 0;
			col.g = 0;
			col.b = 1;
		}
		
		if ((mid.r + mid.g + mid.b)/3 < 0.3) {
			col = col*0.5;
		}
		if ((mid.r + mid.g + mid.b)/3 < 0.6) {
			col = col*0.8;
		}
		*/
			
		col.a = 1;
		
		return col;
    }
    else {
		return mid; 
	}
}

/*
float4 transError(PixelInput input) : COLOR0 {
	float4 col;
	float2 cen = input.uv;
	
	col = tex2D(textureSampler,input.uv);
	float4 offset = tex2D(textureSampler,cen+float2(-0.02,0.05));
	
	if (cen.y < time%5/5 + 0.05 && cen.y > time%5/5 - 0.05) {
		col = abs(offset);	
		col = (col.r + col.b + col.g)/3;
		col += 0.2;
		col *= 2%(cen.x+sin(50*time))+0.5;
		col.a = 1;
	}
	
    return col;
}
*/
 
technique Technique1 {
    pass Pass1 {
		PixelShader = compile ps_2_0 outline();
    }
}
