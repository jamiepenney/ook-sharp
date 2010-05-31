using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace Interpreter
{
	public abstract class JumpInstruction : InstructionBase
	{
		public JumpInstruction MatchingInstruction;
	}

	public class JumpIfZeroInstruction : JumpInstruction
	{

		public override void RunInstruction(InterpreterState state)
		{
			if (state.GetCurrentValue() == 0)
			{
				state.CurrentInstruction = MatchingInstruction;
				Debug.WriteLine(Number + ": 0 Jumping to " + MatchingInstruction.Number);
			}
		}
	}

	public class JumpIfNotZeroInstruction : JumpInstruction
	{
		public override void RunInstruction(InterpreterState state)
		{
			if (state.GetCurrentValue() != 0)
			{
				state.CurrentInstruction = MatchingInstruction;
				Debug.WriteLine(Number + ": !0 Jumping to " + MatchingInstruction.Number);
			}
		}
	}

	public class ReadCharacterInstruction : InstructionBase
	{
		public override void RunInstruction(InterpreterState state)
		{
			char ch = Console.ReadKey().KeyChar;
			state.SetCurrentValueTo(Encoding.ASCII.GetBytes(new []{ch})[0]);
			
		}
	}

	public class PrintCharacterInstruction : InstructionBase
	{
		public override void RunInstruction(InterpreterState state)
		{
			var ch = (byte)state.GetCurrentValue();
			char c = Encoding.ASCII.GetChars(new[] { ch })[0];
			Console.Write(c);
		}
	}

	public enum Direction { Forward, Backwards }

	public class IncrementValueInstruction : InstructionBase
	{
		private readonly Direction direction;

		public IncrementValueInstruction(Direction dir)
		{
			direction = dir;
		}


		public override void RunInstruction(InterpreterState state)
		{
			state.IncrementValue(direction == Direction.Forward ? 1 : -1);
		}
	}

	public class MovePointerInstruction : InstructionBase
	{
		private readonly Direction direction;

		public MovePointerInstruction(Direction dir)
		{
			direction = dir;
		}

		public override void RunInstruction(InterpreterState state)
		{
			state.IncrementPointer(direction == Direction.Forward ? 1 : -1);
		}
	}

	public abstract class InstructionBase : Instruction
	{
		public abstract void RunInstruction(InterpreterState state);
		public int Number { get; set; }
	}

	public interface Instruction
	{
		void RunInstruction(InterpreterState state);
		int Number { get; set; }
	}

	public class UnknownInstructionException : Exception
	{
		public UnknownInstructionException()
		{
		}

		public UnknownInstructionException(string message)
			: base(message)
		{
		}

		public UnknownInstructionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected UnknownInstructionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
