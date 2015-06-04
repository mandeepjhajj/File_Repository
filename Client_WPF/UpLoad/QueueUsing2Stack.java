import java.util.*;

public class QueueUsing2Stack {
	List<Integer> s1 = new Stack<Integer>();
	List<Integer> s2 = new Stack<Integer>();
	public static void main(String[] args) {
		
		
		QueueUsing2Stack q = new QueueUsing2Stack();
		q.enqueue(1);
		q.enqueue(2);
		q.enqueue(3);
		q.enqueue(4);
		q.enqueue(5);
		
		System.out.println("Contents of queue");
		q.showQueue();
		
		q.dequeue();
		q.dequeue();
		q.dequeue();
		
		System.out.println("Queue after deQueue of 3 elements");
		q.showQueue();
		
	}

	public void enqueue(int a)
	{
		Stack<Integer> stk1= (Stack<Integer>)s1;
		stk1.push(a);
	}
	public int dequeue()
	{
		Stack<Integer> stk2= (Stack<Integer>)s2;
		Stack<Integer> stk1= (Stack<Integer>)s1;
		while (!stk1.isEmpty())
			stk2.push(stk1.pop());
		int res = stk2.pop();
		
		//again shifting contents back to stack 1
		while (!stk2.isEmpty())
			stk1.push(stk2.pop());
		
		return res;
	}
	
	public void showQueue()
	{
		for(int a :s1)
			System.out.println(a);
	}
}
