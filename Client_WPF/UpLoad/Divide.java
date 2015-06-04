import java.util.*;


public class Divide {

	
	public int div(int dividend, int divisor)
	{

		int q=1;
		int remainder=0;
		if(dividend == divisor)
		{
			remainder=0;
			return 1;
		}
		else if (dividend < divisor)
		{
			remainder=dividend;
			return 0;
		}
		
		while(divisor<=dividend)
		{
			divisor=divisor << 1;
			q= q << 1;
		}

		divisor = divisor >> 1;
		q = q >> 1;
			
		 /*q = q + div(dividend-divisor, divisor);
		 return q;*/
			
			int answer = 0;
		    // Now deal with the smaller number.
		    while (q != 0)
		    {
		        if (dividend >= divisor)
		        {
		            dividend -= divisor;
		            answer |= q;
		        }
		        q >>= 1;
				divisor >>= 1;
		    }
		    System.out.println("Remainder ="+dividend);
		    return answer;
	}
	public static void main(String[] args) {
	
		Scanner ip = new Scanner(System.in);
		System.out.println("Enter the dividend and divisor");
		int dividend= ip.nextInt();
		int divisor=ip.nextInt();
		
		Divide d = new Divide();
		int res = d.div(dividend,divisor);
		System.out.println("Quotient=" + res);
		//System.out.println("Remainder=" + d.remainder);
		
	}
	

}
