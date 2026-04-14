namespace TrainGame.Model;

/// <summary>
/// A position on the board expressed as fractions of the board image
/// width and height. X grows right, Y grows down (Godot convention).
/// Storing positions in [0,1] space lets the view scale the board freely.
/// </summary>
public readonly record struct NormPos(float X, float Y);
