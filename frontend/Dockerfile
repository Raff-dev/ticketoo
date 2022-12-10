# Use the official Node.js Docker image as the base image
FROM node:14

# Create a new directory in the Docker image for the app
RUN mkdir -p /app

# Set the working directory to the app directory
WORKDIR /app

# Copy the package.json file from the host to the app directory in the Docker image
COPY package.json package-lock.json /app/

# Install the dependencies for the app
RUN npm install --silent

# Copy the rest of the app files from the host to the app directory in the Docker image
COPY . /app

# Expose port 3000 on the Docker container
EXPOSE 3000

# Run the app when the Docker container is started
CMD ["npm", "start"]
