!include FileFunc.nsh

!define APPNAME "USP Cutting Machine"
!define COMPANYNAME "Top Engineering"
!define SOURCEPROJECTNAME "VCM_PickAndPlace"
!define ICONFILE "..\..\TopUI\Resources\Images\topengineering.ico"

!define RELEASEFOLDER "..\..\${SOURCEPROJECTNAME}\bin\Release"
!define EXECUTEFILE "${SOURCEPROJECTNAME}.exe"

!define HELPURL "http://topengnet.co.kr" # "Support Information" link
!define UPDATEURL "http://172.16.161.254" # "Product Updates" link
!define ABOUTURL "http://topengnet.co.kr" # "Publisher" link

# This is the size (in kB) of all the files copied into "Program Files"
!define INSTALLSIZE 148461

InstallDir "D:\TOP\TOPVEQ\Executable"

!macro GetVersionLocal file basedef
!verbose push
!verbose 1
!tempfile _GetVersionLocal_nsi
!tempfile _GetVersionLocal_exe
!appendfile "${_GetVersionLocal_nsi}" 'Outfile "${_GetVersionLocal_exe}"$\nRequestexecutionlevel user$\n'
!appendfile "${_GetVersionLocal_nsi}" 'Section$\n!define D "$"$\n!define N "${D}\n"$\n'
!appendfile "${_GetVersionLocal_nsi}" 'GetDLLVersion "${file}" $2 $4$\n'
!appendfile "${_GetVersionLocal_nsi}" 'IntOp $1 $2 / 0x00010000$\nIntOp $2 $2 & 0x0000FFFF$\n'
!appendfile "${_GetVersionLocal_nsi}" 'IntOp $3 $4 / 0x00010000$\nIntOp $4 $4 & 0x0000FFFF$\n'
!appendfile "${_GetVersionLocal_nsi}" 'FileOpen $0 "${_GetVersionLocal_nsi}" w$\nStrCpy $9 "${N}"$\n'
!appendfile "${_GetVersionLocal_nsi}" 'FileWrite $0 "!define ${basedef}1 $1$9"$\nFileWrite $0 "!define ${basedef}2 $2$9"$\n'
!appendfile "${_GetVersionLocal_nsi}" 'FileWrite $0 "!define ${basedef}3 $3$9"$\nFileWrite $0 "!define ${basedef}4 $4$9"$\n'
!appendfile "${_GetVersionLocal_nsi}" 'FileClose $0$\nSectionend$\n'
!system '"${NSISDIR}\makensis" -NOCD -NOCONFIG "${_GetVersionLocal_nsi}"' = 0
!system '"${_GetVersionLocal_exe}" /S' = 0
!delfile "${_GetVersionLocal_exe}"
!undef _GetVersionLocal_exe
!include "${_GetVersionLocal_nsi}"
!delfile "${_GetVersionLocal_nsi}"
!undef _GetVersionLocal_nsi
!verbose pop
!macroend

!insertmacro GetVersionLocal "${RELEASEFOLDER}\${EXECUTEFILE}" MyVer_

!define VERSION "${MyVer_1}.${MyVer_2}.${MyVer_3}.${MyVer_4}"