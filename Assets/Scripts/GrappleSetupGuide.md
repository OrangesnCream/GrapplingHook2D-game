# Grapple Movement Setup Guide

## Overview
This grapple system provides realistic pendulum physics for swinging and momentum building in your 2D platformer.

## Setup Instructions

### 1. Player Setup
- Add a `Rigidbody2D` component to your player GameObject
- Set the Rigidbody2D to `Dynamic` type
- Add the `GrappleMovement` script to your player
- Add the `Grapplevisual` script to your player (for visual effects)

### 2. Grapple Surfaces
- Create GameObjects for grappleable surfaces (walls, ceilings, etc.)
- Add `Collider2D` components to these surfaces
- Set the appropriate layer mask in the `GrappleMovement` script's `grappleLayerMask` field

### 3. Configuration
Adjust these settings in the inspector:

**Grapple Settings:**
- `grappleRange`: Maximum distance you can grapple (default: 10)
- `swingForce`: How strong the swinging motion is (default: 15)
- `pullForce`: How strongly you're pulled toward the grapple point (default: 25)

**Physics Settings:**
- `maxGrappleLength`: Maximum rope length (default: 8)
- `minGrappleLength`: Minimum rope length (default: 2)
- `momentumMultiplier`: Speed boost when releasing grapple (default: 1.5)
- `airDrag`: Air resistance while grappling (default: 0.98)

**Input:**
- `grappleKey`: Key to attach grapple (default: Mouse0/Left Click)
- `releaseKey`: Key to release grapple (default: Mouse1/Right Click)

## Controls
- **Left Click**: Attach grapple to surface within range
- **Right Click**: Release grapple with momentum boost
- **Mouse Scroll**: Adjust grapple length while grappling

## Features
- **Realistic Pendulum Physics**: Natural swinging motion based on gravity
- **Momentum Building**: Build up speed through swinging
- **Dynamic Length Adjustment**: Extend/retract grapple while swinging
- **Visual Feedback**: Line renderer shows grapple connection
- **Debug Visualization**: Gizmos show grapple range and connection

## Tips
- Use different grapple surfaces at various heights for interesting level design
- Experiment with the physics settings to get the feel you want
- The momentum multiplier allows for exciting traversal mechanics
- Consider adding sound effects for grapple attach/release
- You can modify the visual effects in the `Grapplevisual` script

## Troubleshooting
- Make sure your player has a Rigidbody2D component
- Ensure grapple surfaces have colliders and are on the correct layer
- Check that the camera reference is set correctly
- Verify the grapple range is appropriate for your level scale
