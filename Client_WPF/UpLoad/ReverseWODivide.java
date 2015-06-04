
public class ReverseWODivide {

	public int revInt(int a)
	{
		int ans=0,num=0,rem,divisor=10,factor=1,i=0;
		
		while(factor<a)
		{
			rem=a % divisor-num;
			
			for(i=0;rem>0;i++,rem=rem-factor);
			ans=ans*10+i;
			num=a % divisor;
			divisor=divisor*10;
			factor*=10;
			
		}
		return ans;
	
	}
	public static void main(String[] args) {
	
		ReverseWODivide r = new ReverseWODivide();
		int ans=r.revInt(3412);
		System.out.println(ans);
	}
	/*public int revInt(int in) {
	    int ten = 10, ans = 0, prevDigit = 0, i = 0, nTen, sTen = 1;
	    while (sTen < in) {
	        nTen = (in % ten) - prevDigit;
	        for (i = 0; nTen > 0; i++, nTen -= sTen);
	        ans = ans * 10 + i;
	        prevDigit = (in % ten);
	        ten = ten * 10;
	        sTen = sTen * 10;
	    }
	    return ans;
	}*/
	/*private int revInt(int i) 
	{		
		int r = 0;
		int d = 10;
		int s = 0;
		while(i > 0)
		{
			int remainder = i % 10;
			r = (r * 10) + remainder;
			s = 0;
			while(i > 10)
			{
				i -= d;
				s++;
			}			
			i = s;			
		}
		return r;
	}*/

}
