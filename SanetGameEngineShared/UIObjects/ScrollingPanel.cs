using Sanet.XNAEngine.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sanet.XNAEngine
{
	/// <summary>
	/// Background container
	/// a panel wich contains of 3 parts - one side with rounded corners, scaled central part (1pixel width sprite) and flipped first part
	/// currently supports only vertical layout, but might be expanded
	/// </summary>
	public class ScrollingPanel :GameObject2D
	{
		#region Constructor
		public ScrollingPanel(float width, float height, GameObjectOrientation orientation)
		{
			Height = height;
			Width = width;

			ScrollingDirection = orientation;
			_touchProvider.Scale(width, height);

			_touchProvider.Color = Color.FromNonPremultiplied(new Vector4(1, 0, 0, 0));

			_touchProvider.OnEnter += _touchProvider_OnEnter;
			_touchProvider.OnLeave += _touchProvider_OnLeave;
			_touchProvider.OnClick += _touchProvider_OnClick;

			IsScrollEnabled = true;

			base.AddChild(_touchProvider);
			base.AddChild(_container);
		}
		#endregion

		#region Events
		public event Action<Vector2> OnClick;

		public event Action OnScrolling;

		public event EventHandler CurrentStepChanged;
		#endregion

		#region Fields

		GameSpriteTouchable _touchProvider = new GameSpriteTouchable("EmptyPixel");
		GameSprite _container = new GameSprite("EmptyPixel");

		ScrollBar _verticalScrollBar;

		RasterizerState _rasterizerState = new RasterizerState() { ScissorTestEnable = true };

		Vector2 _lastDistance=Vector2.Zero;
		Vector2 _totalDistance = Vector2.Zero;

		bool _autoLayout = true;

		bool _isFixedStep = false;

		int _minStep;
		int _maxStep;

		int _currentStep=1;
		Vector2 _loc;

		float _height;
		float _width;

		Rectangle _cissorRectangle;
		#endregion

		#region Properties
		public float Height
		{
			get
			{
				return _height / LocalScale.Y;
			}

			private set
			{
				_height = value;
			}
		}
		public float Width
		{
			get;
			private set;
		}

		public float ScrollingHeigth { get; /*private*/ set; }
		public float ScrollingWidth { get; /*private*/ set; }

		public bool CanScroll
		{
			get
			{
				if (!IsScrollEnabled)
					return false;
				if (ScrollingDirection == GameObjectOrientation.Vertical)
					return ScrollingHeigth > Height;
				if (ScrollingDirection == GameObjectOrientation.Horizontal)
					return ScrollingWidth > Width;
				return ((ScrollingHeigth > Height) || (ScrollingWidth > Width));
			}
		}

		public bool SkipInvisible { get; set; }

		public ScrollBar VerticalScrollBar
		{ 
			get
			{
				return _verticalScrollBar;
			}
			set
			{
				if (value!=_verticalScrollBar)
				{
					if (_verticalScrollBar!=null)
					{
						base.RemoveChild(_verticalScrollBar);
						_verticalScrollBar.Unload();
						_verticalScrollBar = null;
					}
					_verticalScrollBar = value;
					base.AddChild(_verticalScrollBar);
				}
			}
		}

		public bool IsPressed
		{
			get
			{
				return _touchProvider.Touch.IsPressed;
			}
		}

		public float MinOffset
		{
			get
			{
				if (ScrollingDirection == GameObjectOrientation.Vertical)
					return -ScrollingHeigth + Height;
				return -ScrollingWidth + Width;
			}
		}

		public bool IsScrollEnabled { get; set; }

		public GameObjectOrientation ScrollingDirection
		{
			get;
			private set;
		}

		public override int Z
		{
			get
			{
				return base.Z;
			}
			set
			{
				base.Z = value;
				foreach (GameSprite sprite in Children)
					sprite.Z = value;
			}
		}

		public bool AutoLayout
		{
			get
			{
				return _autoLayout;
			}
			set
			{
				_autoLayout = value;
			}
		}

		public int ChildrenCount
		{
			get
			{
				return _container.Children.Count;
			}
		}

		public List<GameObject2D> PanelChildren
		{
			get
			{
				return _container.Children;
			}
		}

		public bool IsFixedStep 
		{
			get
			{
				if (ScrollingDirection == GameObjectOrientation.Both)
					return false;
				if (ScrollingDirection == GameObjectOrientation.Horizontal && ItemWidthOverride == 0)
					return false;
				if (ScrollingDirection == GameObjectOrientation.Vertical && ItemHeightOverride == 0)
					return false;
				return _isFixedStep;
			}
			set
			{
				_isFixedStep = value;
			}
		}

		int InnerCurrentStep
		{
			get
			{ return _currentStep; }
			set
			{
				if (value != _currentStep)
				{
					_currentStep = value;
					if (_currentStep > _minStep)
						_currentStep = _minStep;
					if (MinStep != MaxStep && _currentStep < _maxStep)
						_currentStep = _maxStep;
					if (CurrentStepChanged != null)
						CurrentStepChanged(this, null);
				}
				if (ScrollingDirection == GameObjectOrientation.Vertical)
				{

					if (_currentStep != _loc.Y)
						SetReturnAnimation(_loc, new Vector2(_loc.X, _currentStep * ItemHeightOverride), false);
				}
				else if (ScrollingDirection == GameObjectOrientation.Horizontal)
				{

					if (_currentStep != _loc.X)
						SetReturnAnimation(_loc, new Vector2(_currentStep * ItemWidthOverride, _loc.Y), false);
				}
			}
		}

		public int CurrentStep 
		{
			get
			{
				return InnerCurrentStep *-1;
			}
		}
		public int MinStep 
		{
			get
			{
				return _minStep * -1;
			}
			set
			{
				_minStep = value * -1;
			}
		}
		public int MaxStep 
		{
			get
			{
				return _maxStep * -1;
			}
			private set
			{
				_maxStep = value * -1;
			}
		}

		public bool IsStopped
		{
			get
			{
				if (_touchProvider.Touch.IsPressed)
					return false;
				if (_container.PathAnimation != null && _container.PathAnimation.IsPlaying)
					return false;
				return true;
			}
		}

		//used to ovveride calculated item size
		public float ItemHeightOverride
		{ get; set; }
		public float ItemWidthOverride
		{ get; set; }

		public Vector2 ContainerPosition
		{
			get
			{
				return _container.LocalPosition;
			}
		}

		public bool DisableAnimations { get; set; }
		#endregion

		#region Scrolling methods
		void _touchProvider_OnClick()
		{

		}

		void _touchProvider_OnLeave()
		{

		}

		void _touchProvider_OnEnter()
		{

		}
		#endregion

		#region Methods overrides
		public override void AddChild(IGameObject child)
		{
			_container.AddChild(child);
			if (!AutoLayout )
				CheckBoundsForChild((GameObject2D)child);
			if (ChildrenCount == 1)
				InnerCurrentStep = 0;
			MaxStep = ChildrenCount-1;
		}
		public override void RemoveChild(GameObject2D child)
		{
			_container.RemoveChild(child);
			MaxStep = ChildrenCount-1;
		}

		public override void Initialize()
		{
			base.Initialize();
			_touchProvider.OnClick += () =>
			{
				if (OnClick != null /*&& _touchProvider.Touch.Distanse.Length() < 20*/)
				{
					OnClick(_touchProvider.Touch.PressPoint);
				}
			};
		}

		public override void LoadContent(ContentManager contentManager,bool isLocal)
		{
			base.LoadContent(contentManager,isLocal);
			if (isLocal == IsLocalContent)
			{
				for (int i = 1; i <= _container.Children.Count; i++)
				{
					//if auto layout we are arranging children automatically based on scrolling direction
					//in this case defined X,Y position is ignored
					if (AutoLayout)
					{
						var prevChild = _container.Children[i - 1];
						if (i == 1)
							prevChild.Translate(0, 0);
						if (ScrollingDirection == GameObjectOrientation.Vertical)
						{

							if (ItemHeightOverride != 0)
							{
								ScrollingHeigth += ItemHeightOverride;
							}
							else if (prevChild.BoundingRect.HasValue)
							{
								ScrollingHeigth += prevChild.BoundingRect.Value.Height;
							}
							if (i != _container.Children.Count)
							{
								var child = _container.Children[i];
								child.Translate(0, ScrollingHeigth);
							}
						}
						else if (ScrollingDirection == GameObjectOrientation.Horizontal)
						{
							if (ItemWidthOverride != 0)
							{
								ScrollingWidth += ItemHeightOverride;
							}
							else
								if (prevChild.BoundingRect.HasValue)
								{
									ScrollingWidth += prevChild.BoundingRect.Value.Width;
								}
							if (i != _container.Children.Count)
							{
								var child = _container.Children[i];
								child.Translate(ScrollingWidth, 0);
							}
						}
						else
							throw new NotSupportedException("Auto scrolling supports only single scrolling direction");
					}
					else
					{
						var child = _container.Children[i - 1];
						CheckBoundsForChild(child);
					}
				}
			}
		}

		void CheckBoundsForChild(GameObject2D child)
		{
			//var pos = child.LocalPosition;
			if (ItemHeightOverride != 0)
			{
				var item = _container.Children.OrderBy(f => f.LocalPosition.Y).Last();
				//ScrollingHeigth = ItemHeightOverride * _container.Children.Count;
				ScrollingHeigth = item.LocalPosition.Y + ItemHeightOverride;
			}
			else if (ItemWidthOverride != 0)
			{
				//pos.X = ScrollingWidth;
				ScrollingWidth = ItemWidthOverride * _container.Children.Count;
			}
			else if (child.BoundingRect.HasValue)
			{
				var bottom = child.BoundingRect.Value.Bottom + child.LocalPosition.Y;
				var right = child.BoundingRect.Value.Right + child.LocalPosition.X;
				if (ScrollingHeigth < bottom)
					ScrollingHeigth = bottom;
				if (ScrollingWidth < right)
					ScrollingWidth = right;
			}
			//child.Translate(pos);
		}

		public float ScrollingIndex
		{
			get
			{
				return (ScrollingDirection== GameObjectOrientation.Vertical)?Height/ScrollingHeigth:Width/ScrollingWidth;
			}
		}

		public override void Draw(RenderContext renderContext)
		{
			//we need to draw children only in visible part of scrolling panel
			renderContext.SpriteBatch.End();
			renderContext.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
				null, null, _rasterizerState);
			renderContext.SpriteBatch.GraphicsDevice.ScissorRectangle = _cissorRectangle;
			base.Draw(renderContext);
			renderContext.SpriteBatch.GraphicsDevice.ScissorRectangle = Rectangle.Empty;
			renderContext.SpriteBatch.End();
			renderContext.SpriteBatch.Begin();
		}

		public override void Update(RenderContext renderContext)
		{
			_touchProvider.Touch.Update(renderContext);

			if (IsPressed)
			{
				//TouchInput.AbortForLoop = _touchProvider.Touch.LastDistance!=Vector2.Zero;
			}

			_cissorRectangle = new Rectangle(
				(int)(WorldPosition.X),
				(int)(WorldPosition.Y),
				(int)(Width * renderContext.DeviceScale.X),
				(int)(_height * renderContext.DeviceScale.Y)
			);
			base.Update(renderContext);

			//for many items this is useful for performance
			if (SkipInvisible)
			{
				List<int> visibleObjs = new List<int> ();
				for (int i =0;i<_container.Children.Count;i++)
				{
					var obj = _container.Children [i];
					obj.CanDraw = false;
					if (obj.BoundingRect.HasValue)
					{

						if (_cissorRectangle.Intersects(obj.BoundingRect.Value) /*|| _cissorRectangle.Contains(wPos)*/)
						{
							obj.CanDraw = true;
							visibleObjs.Add (i);
						}
					}
					else if (ScrollingDirection== GameObjectOrientation.Vertical)
					{
						var rect = new Rectangle((int)obj.LocalPosition.X, (int)obj.LocalPosition.Y, (int)Width, (int)ItemHeightOverride);
						if (_cissorRectangle.Intersects(rect) )
						{
							obj.CanDraw = true;
							visibleObjs.Add(i);
						}
					}
				}
				if (visibleObjs.Count > 0)
				{
					var top = Math.Max(0, visibleObjs[0] - 2);
					var bottom = Math.Min(_container.Children.Count - 1, visibleObjs.Last() + 2);
					for (var j = top; j < visibleObjs.First(); j++)
						_container.Children[j].CanDraw = true;
					for (var j = visibleObjs.Last(); j <= bottom; j++)
						_container.Children[j].CanDraw = true;
				}
				else
				{
					foreach (var c in _container.Children)
					{
						c.CanDraw = true;
					}
				}
			}

			if (!CanDraw)
				return;
			if (Parent != null && !Parent.CanDraw)
				return;

			var canScroll= CanScroll;
			_loc = _container.LocalPosition;
			//update ScrollBars
			//vertical
			if (VerticalScrollBar!=null && ScrollingDirection== GameObjectOrientation.Vertical)
			{
				VerticalScrollBar.CanDraw = canScroll;
				if (canScroll)
				{
					VerticalScrollBar.Length = Height;
					VerticalScrollBar.ScrollLength = VerticalScrollBar.Length * ScrollingIndex;
					VerticalScrollBar.Translate(Width - VerticalScrollBar.Width, 0);
					VerticalScrollBar.UpdatePosition(_loc.Y / (Height - ScrollingHeigth));
				}
			}
			//horizontal TODO

			if (!IsScrollEnabled)
				return;


			if (canScroll)
			{
				//if pressed
				if (_touchProvider.Touch.IsPressed /*&& _touchProvider.Touch.Distance.Length() > 2*/)
				{
					_lastDistance = _touchProvider.Touch.LastDistance / renderContext.DeviceScale;
					_totalDistance += _lastDistance;

					if (ScrollingDirection == GameObjectOrientation.Vertical)
					{
						_loc.Y += _lastDistance.Y;
					}
					else if (ScrollingDirection == GameObjectOrientation.Horizontal)
					{
						_loc.X += _lastDistance.X;
					}
					else
						_loc += _lastDistance;
					_container.Translate(_loc);

					if (OnScrolling != null && _lastDistance.Length() > 1/*&& _touchProvider.Touch.PressTime > TouchInput.MaximumClickTime*/)
					{
						OnScrolling();
					}
				}
				else
				{
					if (_container.PathAnimation != null && _container.PathAnimation.IsPlaying)
						return;

					if (IsFixedStep)
					{
						//FixedStep means scrolling should stop so that one item is always in the middle
						//this has sense only when scrolling in one direction
						if (ScrollingDirection == GameObjectOrientation.Vertical)
						{
							if (_loc.Y != InnerCurrentStep)
							{
								float closest = (float)Math.Round((_loc.Y+_totalDistance.Y) / ItemHeightOverride, 0);
								InnerCurrentStep = (int)closest;

							}
						}
						else if (ScrollingDirection == GameObjectOrientation.Horizontal)
						{
							if (_loc.X != InnerCurrentStep)
							{
								float closest = (float)Math.Round((_loc.X +_totalDistance.X)/ ItemWidthOverride, 0);
								InnerCurrentStep = (int)closest;

							}
						}
					}
					else
					{//just check if we scrolled out of borders and animate back
						var minOffset = MinOffset;
						var anLoc = _loc + _lastDistance ;
						if (ScrollingDirection == GameObjectOrientation.Vertical)
						{
							if (anLoc.Y < minOffset)
							{
								SetReturnAnimation(_loc, new Vector2(_loc.X, minOffset), false);
							}
							else if (anLoc.Y > 0)
							{
								SetReturnAnimation(_loc, new Vector2(_loc.X, 0), false);
							}
							else if (_lastDistance != Vector2.Zero)
							{
								SetReturnAnimation(_loc, new Vector2(_loc.X, anLoc.Y), false);
							}
						}
						else if (ScrollingDirection == GameObjectOrientation.Horizontal)
						{
							if (anLoc.X < minOffset)
							{
								SetReturnAnimation(_loc, new Vector2(minOffset, _loc.Y), false);
							}
							else if (anLoc.X > 0)
							{
								SetReturnAnimation(_loc, new Vector2(0, _loc.Y), false);
							}
							else if (_lastDistance != Vector2.Zero)
							{
								SetReturnAnimation(_loc, new Vector2(anLoc.X, _loc.Y), false);
							}
						}
						else
						{
							Vector2 toGo = anLoc;
							var minOffsetY = (-ScrollingHeigth + Height);
							var minOffsetX = (-ScrollingWidth + Width);

							if (anLoc.Y < minOffsetY)
							{
								toGo.Y = minOffsetY;
							}
							else if (anLoc.Y > 0)
							{
								toGo.Y = 0;
							}

							if (anLoc.X < minOffsetX)
							{
								toGo.X = minOffsetX;
							}
							else if (anLoc.X > 0)
							{
								toGo.X = 0;
							}


							if (toGo != _loc)
							{
								SetReturnAnimation(_loc, toGo, false);
							}
						}
						_lastDistance = Vector2.Zero;
					}
					_totalDistance = Vector2.Zero;
				}
			}

		}

		void SetReturnAnimation(Vector2 pos1, Vector2 pos2, bool force
			#if !WP7
			=false
			#endif
		)
		{
			if (DisableAnimations && !force)
			{ 
				_container.Translate(pos2);
			}
			else
			{
				var speed = (pos1 - pos2).Length() * 5;

				var point1 = new PathAnimationPoint(pos1, Vector2.One, 0, speed);
				var point2 = new PathAnimationPoint(pos2, Vector2.One, 0, speed);
				_container.PathAnimation = new PathAnimation(new List<PathAnimationPoint> { point1, point2 });
				//_container.PathAnimation.Completed += () => SceneManager.SetActiveScene(ScenePrefix + (pageNumber).ToString());
				_container.PathAnimation.PlayAnimation();
			}
		}

		public void Clear()
		{
			var l = new List<GameObject2D>();
			l.AddRange(_container.Children);
			foreach (var obj in l)
			{
				_container.RemoveChild(obj);
			}
			l.Clear();
			ScrollingHeigth = 0;
			ScrollingWidth = 0;
			l = null;
		}

		public void ScrollTo(int step)
		{
			if (!IsFixedStep)
				return;
			InnerCurrentStep = step;
			if (ScrollingDirection== GameObjectOrientation.Vertical)
				SetReturnAnimation(_container.LocalPosition, new Vector2(0, ItemHeightOverride * step * -1), false);
			else if (ScrollingDirection == GameObjectOrientation.Horizontal)
				SetReturnAnimation(_container.LocalPosition, new Vector2(ItemWidthOverride * step * -1, 0), false);
		}

		public void ScrollTo(float length)
		{
			if (ScrollingDirection == GameObjectOrientation.Vertical)
				SetReturnAnimation(_container.LocalPosition, new Vector2(0, -length), true);
			else if (ScrollingDirection == GameObjectOrientation.Horizontal)
				SetReturnAnimation(_container.LocalPosition, new Vector2(-length, 0), true);
		}
		#endregion


	}
}
