# Complete Guide: Applying MVC Architecture in Unity

## Overview
This guide shows you how to set up and use the MVC (Model-View-Controller) architecture for your cube system in Unity.

---

## Part 1: Understanding the MVC Structure

### **Model (CubeModel.cs)** - Pure Data
- Location: `Assets/Scripts/Models/CubeModel.cs`
- **NOT a MonoBehaviour** - Pure C# class
- Contains: Grid position, cube type, state data
- No Unity dependencies, easy to test

### **View (CubeView.cs)** - Visual Representation
- Location: `Assets/Scripts/Views/CubeView.cs`
- **IS a MonoBehaviour** - Attached to GameObject
- Contains: Animations, visual updates, rendering
- Listens to Model events and updates visuals

### **Controller (CubeController.cs)** - Business Logic
- Location: `Assets/Scripts/Controllers/CubeController.cs`
- **IS a MonoBehaviour** - Attached to GameObject
- Contains: Game logic, coordinates Model and View
- Public API for external systems

### **Factory (CubeFactory.cs)** - Creation Logic
- Location: `Assets/Scripts/Factory/CubeFactory.cs`
- **IS a MonoBehaviour** - Singleton in scene
- Contains: Cube instantiation and initialization
- Used by managers to create cubes

---

## Part 2: Setting Up the Cube Prefab

### **Step 1: Open the Cube Prefab**
1. In Unity Project window, navigate to `Assets/Prefab/`
2. Double-click `cubeSimple.prefab` to open it in Prefab Mode

### **Step 2: Remove Old Components**
1. Select the cube GameObject in the Hierarchy
2. In the Inspector, find the old **Cube** component
3. Click the three dots (⋮) → Remove Component
4. **Keep these components:**
   - Transform
   - Mesh Filter
   - Mesh Renderer
   - CubeColorManager
   - CubeMaterialController

### **Step 3: Add New MVC Components**
1. Click **Add Component** button
2. Search for and add: **CubeController**
3. Click **Add Component** again
4. Search for and add: **CubeView**

### **Step 4: Wire Up References**
1. Select the cube GameObject
2. In the **CubeController** component:
   - Find the `View` field
   - Drag the **CubeView** component (from the same GameObject) into this field
   - OR click the circle icon and select CubeView

### **Step 5: Save the Prefab**
1. Click the **< (back arrow)** at the top of the Hierarchy to exit Prefab Mode
2. Unity will automatically save your changes

**Your prefab should now have:**
```
cubeSimple (GameObject)
├── Transform
├── Mesh Filter
├── Mesh Renderer
├── CubeColorManager
├── CubeMaterialController
├── CubeView          ← NEW
└── CubeController    ← NEW (with View reference set)
```

---

## Part 3: Setting Up the Scene

### **Step 1: Create CubeFactory GameObject**
1. In your scene Hierarchy, right-click → Create Empty
2. Rename it to **"CubeFactory"**
3. With CubeFactory selected, click **Add Component**
4. Search for and add: **CubeFactory**

### **Step 2: Assign Cube Prefab to Factory**
1. Select the **CubeFactory** GameObject
2. In the Inspector, find the **CubeFactory** component
3. Find the `Cube Prefab` field
4. Drag `Assets/Prefab/cubeSimple.prefab` into this field
5. OR click the circle icon and select cubeSimple

### **Step 3: Set Up GroundGenerator**
1. Find or create your **GroundGenerator** GameObject in the scene
2. Select it and find the **GroundGenerator** component in Inspector
3. Find the `Cube Factory` field
4. Drag the **CubeFactory** GameObject from Hierarchy into this field

### **Step 4: Set Up CubeLevelManager**
1. Find or create your **CubeLevelManager** GameObject in the scene
2. Select it and find the **CubeLevelManager** component in Inspector
3. Find the `Cube Factory` field
4. Drag the **CubeFactory** GameObject from Hierarchy into this field

### **Step 5: Set Up LevelSequenceController**
1. Find your **LevelSequenceController** GameObject
2. Make sure it has references to:
   - GroundGenerator
   - CubeLevelManager (or CubeLevelGenerator)
   - PressureManager

**Your scene hierarchy should look like:**
```
Scene
├── CubeFactory          ← NEW (with cubeSimple prefab assigned)
├── GroundGenerator      ← (with CubeFactory reference)
├── CubeLevelManager     ← (with CubeFactory reference)
├── LevelSequenceController
├── Player
└── ... (other objects)
```

---

## Part 4: How the MVC System Works at Runtime

### **Creation Flow:**
```
1. LevelSequenceController starts
   ↓
2. Calls GroundGenerator.SpawnSequence()
   ↓
3. GroundGenerator calls CubeFactory.CreateCube()
   ↓
4. CubeFactory:
   - Instantiates the prefab
   - Gets CubeController component
   - Calls controller.Initialize()
   ↓
5. CubeController.Initialize():
   - Creates new CubeModel (pure C# object)
   - Initializes model with data
   - Tells CubeView to initialize with model
   ↓
6. CubeView.Initialize():
   - Subscribes to model events
   - Sets initial visual state
   ↓
7. Cube is ready to use!
```

### **Interaction Flow:**
```
External System (e.g., PlayerController)
   ↓
Calls CubeController.Select()
   ↓
CubeController updates CubeModel
   ↓
CubeModel fires OnTypeChanged event
   ↓
CubeView receives event and updates visuals
```

---

## Part 5: Using the MVC System in Code

### **Example 1: Creating Cubes (in GroundGenerator)**
```csharp
public IEnumerator SpawnSequence(int width, int depth)
{
    for (int x = 0; x < width; x++)
    {
        for (int z = 0; z < depth; z++)
        {
            Vector2Int pos = new Vector2Int(x, z);
            
            // Factory creates and initializes the cube
            CubeController cube = cubeFactory.CreateCube(
                pos, 
                CubeModel.CubeType.Gray,
                transform
            );
            
            if (cube != null)
            {
                groundCubes[pos] = cube;
            }
            
            yield return null;
        }
    }
}
```

### **Example 2: Interacting with Cubes (in PlayerController)**
```csharp
void OnCubeSelect(InputAction.CallbackContext ctx)
{
    if (!ctx.performed) return;
    
    // Get cube from ground generator
    CubeController groundCube = groundGenerator.GetGroundCubeUnderWorldPos(transform.position);
    
    if (groundCube != null)
    {
        if (selectedCube == groundCube)
        {
            // Unselect through controller
            groundCube.Unselect();
            selectedCube = null;
        }
        else
        {
            if (selectedCube != null)
            {
                selectedCube.Unselect();
            }
            // Select through controller
            groundCube.Select();
            selectedCube = groundCube;
        }
    }
}
```

### **Example 3: Accessing Cube Data**
```csharp
// Access model data through controller
CubeController cube = GetCubeAt(position);

// Read data
Vector2Int gridPos = cube.Model.GridPos;
CubeModel.CubeType type = cube.Model.ActualType;
bool isRolling = cube.Model.IsRolling;

// Modify through controller methods
cube.ChangeType(CubeModel.CubeType.Green);
cube.Roll(Vector2Int.up);
cube.Select();
```

---

## Part 6: Common Issues and Solutions

### **Issue 1: "NullReferenceException: Object reference not set"**
**Cause:** CubeFactory reference not set in managers
**Solution:** 
1. Select GroundGenerator/CubeLevelManager in scene
2. Drag CubeFactory GameObject to the `Cube Factory` field

### **Issue 2: "CubeController.View is null"**
**Cause:** View reference not set in prefab
**Solution:**
1. Open cubeSimple prefab
2. Select the cube GameObject
3. In CubeController component, drag CubeView to the `View` field
4. Save prefab

### **Issue 3: "Cube prefab is missing CubeController component"**
**Cause:** Old Cube component still on prefab instead of new MVC components
**Solution:**
1. Open cubeSimple prefab
2. Remove old Cube component
3. Add CubeController and CubeView components
4. Wire up references

### **Issue 4: Cubes don't appear in scene**
**Cause:** CubeFactory doesn't have prefab assigned
**Solution:**
1. Select CubeFactory GameObject in scene
2. Drag cubeSimple.prefab to `Cube Prefab` field

---

## Part 7: Testing Your Setup

### **Quick Test Checklist:**

1. **Prefab Check:**
   - [ ] Open cubeSimple.prefab
   - [ ] Has CubeController component
   - [ ] Has CubeView component
   - [ ] CubeController.View field is assigned
   - [ ] Has CubeColorManager component

2. **Scene Check:**
   - [ ] CubeFactory GameObject exists
   - [ ] CubeFactory has cubeSimple prefab assigned
   - [ ] GroundGenerator has CubeFactory reference
   - [ ] CubeLevelManager has CubeFactory reference

3. **Runtime Test:**
   - [ ] Press Play
   - [ ] Cubes spawn correctly
   - [ ] No errors in Console
   - [ ] Cubes respond to interactions

---

## Part 8: Benefits You'll See

### **Before MVC (Old Cube.cs):**
- ❌ Everything in one giant class
- ❌ Hard to test
- ❌ Tight coupling
- ❌ Difficult to modify

### **After MVC:**
- ✅ Clear separation of concerns
- ✅ Easy to test (Model is pure C#)
- ✅ Loose coupling through events
- ✅ Easy to modify and extend
- ✅ Reusable components
- ✅ Better code organization

---

## Part 9: Next Steps

Once you have the basic MVC setup working:

1. **Refactor other systems:**
   - Apply MVC to PressureManager
   - Apply MVC to LevelSequenceController
   - Apply MVC to PlayerController

2. **Add new features easily:**
   - New cube types (just add to CubeModel.CubeType enum)
   - New animations (add to CubeView)
   - New behaviors (add to CubeController)

3. **Write tests:**
   - Unit test CubeModel (pure C#, no Unity needed)
   - Integration test CubeController
   - Visual test CubeView

---

## Summary

**The key to applying MVC in Unity:**

1. **Model** = Pure data (no MonoBehaviour)
2. **View** = Visual representation (MonoBehaviour on GameObject)
3. **Controller** = Business logic (MonoBehaviour on GameObject)
4. **Factory** = Creation logic (MonoBehaviour singleton in scene)

**Setup steps:**
1. Update cube prefab with MVC components
2. Create CubeFactory in scene
3. Wire up references in managers
4. Test and verify

**Remember:** The Controller is the public API. External systems should only interact with the Controller, never directly with Model or View.

---

Good luck with your MVC refactoring! 🎮
