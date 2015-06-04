
public class ReverseString {

	public static void main(String[] args) {
		ReverseString r = new ReverseString();
		String rev = r.reverse(12344);
		System.out.println("Reverse is "+rev);
	}
	public String reverse(int a)
	{
		String rev="";
		while(a!=0)
		{
			int temp = a%10;
			a = a/10;
			rev+=temp;
		}
		return rev;
	}

}
