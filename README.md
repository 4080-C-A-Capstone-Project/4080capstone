# Cryptic

This capstone project aims to aid in encryption and decryption of text and files through 4 different means. 
These include:
1. Caesar Cipher
2. XOR
3. AES - Advanced Encryption System
4. OpenPGP

The program uses Avalonia GUI with .Net 8.0.0, which can be used with Windows and MacOS. 

When attempting to run the program:
If you are using the terminal to run:
1. Type dotnet build within the project folder.
2. After the build completes, then type dotnet run to initiate the encryption and decryption program.

When you open the program, you will need to set a username to encrypt and decrypt files. After setting the username, you can encrypt and decrypt text and files. If you wish to encrypt text, you can do that by clicking the enter text button, typing the text you wish to encrypt, and clicking the save button. After that, you can select your encryption method like Caesar, XOR, AES, or OpenPGP. If you are using the Caesar Cipher, XOR, or AES, you then click encrypt. You then can save either the .cis file for the Caesar Cipher, the .xor file for XOR, or .aes file for AES to your files. You then get a generated key you can use to decrypt the encrypted text or file.

When you want to decrypt a file, you need to get the encrypted file, .cis, .xor, or .aes file and select it. Once you do, you then need to select the appropriate decryption method, .cis files need Caesar Cipher, .xor files need XOR, and .aes files need AES. After that, you need the decryption key that was given within the encryption menu. After that, you will get the decrypted file or text and then can examine it.

For OpenPGP, it works somewhat differently. You need to generate a Keyring that will have a public and a private key for you to use. To do this, click on the Keyring button and then you will see the buttons to generate a new key or import a key. If you choose to import a key, you need to import a .asc file to use as the key. If you choose to create a new key, you set up the identity, which is the name or email addresss, and the passphrase to protect the key. 

After that, when you choose to use OpenPGP to encrypt a file, you are then able to select a key rather than have a key be generated for you. You can then encrypt the text or file that way as a .pgp file. When you want to decrypt the file, you need to select the appropriate .pgp file and then get the right key to unlock it, along with the private key password. That will allow you to access the text or file.

TODO (Any clarity issues or problems?)
