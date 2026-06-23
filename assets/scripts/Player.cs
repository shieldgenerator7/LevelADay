// 2026-06-22: made from template, following tutorial: https://youtu.be/oED12Mo2018
using Godot;
using System;

public partial class Player : CharacterBody2D
{
	
	[Export]
	public AnimatedSprite2D animatedSprite2D {get; set;}
	
	public const float Speed = 300.0f;
	public const float JumpVelocity = -850.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		// Add animation
		if (velocity.X > 1 || velocity.X < -1){
			animatedSprite2D.Animation = "walk";
		}
		else{
			animatedSprite2D.Animation = "idle";
		}

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
			animatedSprite2D.Animation = "jump";
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = Input.GetVector("left", "right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
