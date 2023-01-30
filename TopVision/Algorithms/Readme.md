# Guild to create new vision algorithm (aka "vision process")
## 1. Create new vision algorithm class name (ex: Binarization)

- Create new file Binarization.cs
- Inherit to class "VisionProcessBase"
- Implement "DIPFunction" method inside the class

## 2. Create new vision parameter & vision result class name (ex: BinParameter & BinResult)

- Create 2 class BinParameter & BinResult inside "Binarization.cs" file
- Inherit BinParameter to class "VisionParameterBase"
- Inherit BinResult to class "VisionResultBase"
- Note: if new vision algorithm has no extra parameter or result, then BinParameter or BinResult is not required