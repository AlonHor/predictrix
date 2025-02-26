# Predictrix

Predictrix is an API for a game that involves predicting daily events with friends. This is currently a work in progress.

## Table of Contents

- [Description](#description)
- [Technologies](#technologies)
- [Setup](#setup)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Description

Predictrix allows users to predict daily events and compete with friends. The goal is to provide a fun and engaging way to make predictions and see who among your friends is the best predictor.

## Technologies

- **C#**: The primary language used for developing the API.
- **Dockerfile**: Used for containerizing the application.

## Setup

To set up the project locally, follow these steps:

1. Clone the repository:
   ```bash
   git clone https://github.com/AlonHor/predictrix.git
   ```

2. Navigate to the project directory:
   ```bash
   cd predictrix
   ```

3. Build the project using your preferred method (e.g., Visual Studio or `dotnet` CLI).

4. (Optional) Build the Docker image:
   ```bash
   docker build -t predictrix .
   ```

## Usage

To start using the Predictrix API, run the application using your preferred method. If using Docker, you can run the following command:

```bash
docker run -p 8000:80 predictrix
```

Once the application is running, you can interact with the API using your preferred API client (e.g., Postman).

## Contributing

We welcome contributions to the Predictrix project. To contribute, please follow these steps:

1. Fork the repository.
2. Create a new branch with a descriptive name:
   ```bash
   git checkout -b my-feature-branch
   ```
3. Make your changes.
4. Commit your changes with a meaningful commit message:
   ```bash
   git commit -m "Add new feature"
   ```
5. Push your changes to your fork:
   ```bash
   git push origin my-feature-branch
   ```
6. Open a pull request to the `main` branch of the original repository.

## License

This project is licensed under the MIT License.
