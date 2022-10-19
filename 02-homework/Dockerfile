FROM nginx:latest

RUN touch file1
RUN echo "Hello" > file1
RUN echo "Additional layer" > file2

CMD ["nginx", "-g", "daemon off;"]

