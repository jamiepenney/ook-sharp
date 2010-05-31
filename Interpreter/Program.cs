using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Interpreter
{
	class Program
	{
		static void Main()
		{
			string text = File.ReadAllText("fib.ook");

			Microsoft.FSharp.Collections.List<string> instructions = Parser.parse(text);

			StringBuilder sb = new StringBuilder();
			foreach (var inst in instructions)
				sb.AppendLine(inst);

			Console.WriteLine(sb.ToString());

			File.WriteAllText("log.txt", sb.ToString());

			LinkedList<Instruction> instructionList = CreateInstructionList(instructions);

			InterpreterState state = new InterpreterState();
			state.AddInstructionList(instructionList);

			while(state.Done == false)
			{
				state.ExecuteNextInstruction();
			}

			Console.ReadKey();
		}

		private static LinkedList<Instruction> CreateInstructionList(IEnumerable<string> instructions)
		{
			LinkedList<Instruction> instructionList = new LinkedList<Instruction>();

			Stack<JumpInstruction> jumpStack = new Stack<JumpInstruction>();

			int i = 1;
			foreach(var instrText in instructions)
			{
				switch (instrText)
				{
					case "increment-pointer":
						instructionList.AddLast(new MovePointerInstruction(Direction.Forward) {Number = i++});
						break;
					case "decrement-pointer":
						instructionList.AddLast(new MovePointerInstruction(Direction.Backwards) { Number = i++ });
						break;
					case "increment-value":
						instructionList.AddLast(new IncrementValueInstruction(Direction.Forward) { Number = i++ });
						break;
					case "decrement-value":
						instructionList.AddLast(new IncrementValueInstruction(Direction.Backwards) { Number = i++ });
						break;
					case "stdout":
						instructionList.AddLast(new PrintCharacterInstruction { Number = i++ });
						break;
					case "stdin":
						instructionList.AddLast(new ReadCharacterInstruction { Number = i++ });
						break;
					case "jump-ifzero":
						JumpIfZeroInstruction instruction = new JumpIfZeroInstruction { Number = i++ };
						instructionList.AddLast(instruction);
						jumpStack.Push(instruction);
						break;
					case "jump-ifnotzero":
						JumpIfNotZeroInstruction notZeroInstruction = new JumpIfNotZeroInstruction { Number = i++ };
						instructionList.AddLast(notZeroInstruction);
						
						var ifZeroInstruction = jumpStack.Pop();
						ifZeroInstruction.MatchingInstruction = notZeroInstruction;
						notZeroInstruction.MatchingInstruction = ifZeroInstruction;
						break;
					default:
						throw new UnknownInstructionException(instrText);
				}
			}

			return instructionList;
		}
	}

	//    Ook. Ook?   Increment the pointer 
	//    Ook? Ook.   Decrement the pointer 
	//    Ook. Ook.   Increment the value at the pointer 
	//    Ook! Ook!   Decrement the value at the pointer 
	//    Ook! Ook.   Write the value at the pointer to standard output 
	//    Ook. Ook!   Read a value from standard input and store it at the pointer 
	//    Ook! Ook?   If the value at the pointer is zero, jump past the matching Ook? Ook! 
	//    Ook? Ook!   Jump to the matching Ook! Ook?

	public class InterpreterState
	{
		private readonly List<int> internalArray = new List<int>();
		private int currentPointerLocation;

		private readonly LinkedList<Instruction> instructionList = new LinkedList<Instruction>();
		public Instruction CurrentInstruction;
		public bool Done { get; private set; }

		public void AddInstructionList(IEnumerable<Instruction> instructions)
		{
			foreach (var ins in instructions)
				instructionList.AddLast(ins);
			CurrentInstruction = instructionList.First.Value;
		}

		public void ExecuteNextInstruction()
		{
			Debug.WriteLine(CurrentInstruction.Number + ":");
			
			CurrentInstruction.RunInstruction(this);
			
			var next = instructionList.Find(CurrentInstruction).Next;
			if(next == null)
			{
				Done = true;
				return;
			}
			CurrentInstruction = next.Value;
		}

		public void IncrementPointer(int amount)
		{
			if (currentPointerLocation + amount < 0)
				throw new InvalidInterpreterStateException();

			currentPointerLocation += amount;
		}

		public int GetCurrentValue()
		{
			SortArrayOut();

			return internalArray[currentPointerLocation];
		}

		public void IncrementValue(int i)
		{
			SortArrayOut();

			internalArray[currentPointerLocation] += i;
		}

		internal int GetValueAt(int p)
		{
			SortArrayOut();

			return internalArray[p];
		}

		public void SetCurrentValueTo(byte b)
		{
			SortArrayOut();

			internalArray[currentPointerLocation] = b;
		}

		private void SortArrayOut()
		{
			while (internalArray.Count < currentPointerLocation + 1)
				internalArray.Add(0);
		}
	}

	public class InvalidInterpreterStateException : Exception
	{
		public InvalidInterpreterStateException()
		{
		}

		public InvalidInterpreterStateException(string message) : base(message)
		{
		}

		public InvalidInterpreterStateException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidInterpreterStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
