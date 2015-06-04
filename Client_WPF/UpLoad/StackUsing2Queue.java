import java.util.*;

public class StackUsing2Queue {
	Queue<Integer> q1 = new LinkedList<Integer>();
	Queue<Integer> q2 = new LinkedList<Integer>();
	public static void main(String[] args) {
		
		
		StackUsing2Queue s1 = new StackUsing2Queue();
		System.out.println("Adding elemts in stack ");
		s1.push(1);
		s1.push(2);
		s1.push(3);
		s1.push(4);
		s1.push(5);
		
		s1.showStack();
		
		System.out.println("removing elements from stack");
		int a = s1.pop();
		s1.changeref();
		System.out.println("removed elemts is "+a);
		
		System.out.println("remaining elements in stack ");
		
		s1.showStack();
		
		int c = s1.pop();
		s1.changeref();
		System.out.println("removed elemts is "+c);
		
		System.out.println("remaining elements in stack ");
		
		s1.showStack();
	}
	public void showStack()
	{
		for(int b :q1)
		{
			System.out.println("Elements in stack "+b);
		}
	}
	
	public void push(/*Queue<Integer> q1cpy,*/ int a)
	{
		q1.add(a);
	}
	public int pop(/*Queue<Integer> q1cpy, Queue<Integer> q2cpy*/)
	{
		while(q1.size()!=1)
		{
		  q2.add(q1.remove());
		}
		return q1.remove();
		
	}
	public void changeref()
	{
		Queue<Integer> temp = q1;
		q1=q2;
		q2=temp;
	}
	

}
