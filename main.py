class Student:
    def __init__(self, name, username, password, address, city, state):
        self._name = name,
        self._username = username
        self._password = password
        self._address = address
        self._city = city
        self._state = state


class Tutor:
    def __init__(self, name, username, password, address, city, state, phone_number, email):
        self._name = name,
        self._username = username
        self._password = password
        self._address = address
        self._city = city
        self._state = state
        self._phone_number = phone_number
        self._email = email


def register():
    command = input("Student or Tutor: ")
    if command == "Student":
        name = input("Enter Your Full Name: ")
        username = input("Select a Username: ")
        password = input("Create a Password: ")
        address = input("Enter Your Street Address: ")
        city = input("Enter Your City: ")
        state = input("Enter Your State: ")
        name = Student(name, username, password, address, city, state)

    if command == "Tutor":
        name = input("Enter Your Full Name: ")
        username = input("Select a Username: ")
        password = input("Create a Password: ")
        address = input("Enter Your Street Address: ")
        city = input("Enter Your City: ")
        state = input("Enter Your State: ")
        phone_number = input("Enter Your Phone Number: ")
        contact_email = input("Enter Your Email: ")
        name = Tutor(name, username, password, address, city, state, phone_number, contact_email)


def login(username, password):
    if username not in databaseU or password not in databaseP:
        print("Username or Password is incorrect, please re-enter or register.")
        command = input()
        if command == "Register":
            register()
        if command == "Login":
            login(input("Username: "), input("Password: "))
    runapp()


def runapp():
    print("hi")


if __name__ == '__main__':
    databaseU = []
    databaseP = []

    print('Welcome, Please Login')
    login(input("Username: "), input("Password: "))
